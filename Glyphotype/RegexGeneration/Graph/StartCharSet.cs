namespace Glyphotype.RegexGeneration.Graph;

/// <summary>
/// The characters a <see cref="RegexGraph"/>'s match can begin with - so the Tokenizer can skip a type at any
/// position whose character isn't among them, without paying for a regex call that would fail on its first
/// character anyway (nearly every attempt, in practice).
/// <para>
/// Derived from the node graph rather than by parsing the built regex, and deliberately conservative: anything
/// the analysis can't pin down (a dynamic's wildcard, a nib opening with a group or class, an optional first atom)
/// makes the set <see cref="IsAny"/>, and every other imprecision only ever adds characters. So it can cost a
/// wasted attempt, never a missed match.
/// </para>
/// </summary>
public sealed class StartCharSet
{
    public static readonly StartCharSet Any = new(null);

    readonly HashSet<char> _chars;
    readonly bool[] _ascii;

    StartCharSet(HashSet<char> chars)
    {
        _chars = chars;

        if (chars is null)
            return;

        _ascii = new bool[128];

        foreach (var c in chars.Where(c => c < 128))
            _ascii[c] = true;
    }

    /// <summary>Whether a match can begin with any character at all, i.e. there's nothing to filter on.</summary>
    public bool IsAny => _chars is null;

    /// <summary>The characters a match can begin with, or null when <see cref="IsAny"/>.</summary>
    public IReadOnlyCollection<char> Chars => _chars;

    public bool CanStartWith(char c) =>
        _chars is null || (c < 128 ? _ascii[c] : _chars.Contains(c));

    public static StartCharSet Of(GlyphNode rootNode) =>
        Analyze(rootNode) is { Chars: { } chars, Nullable: false } ? new(chars) : Any;

    public override string ToString() =>
        IsAny ? "any" : string.Concat(_chars.OrderBy(x => x));

    /// <summary>What a node's match can begin with: <see cref="Chars"/> null means anything; <see cref="Nullable"/> means it can also match nothing, so whatever follows it can begin the match too.</summary>
    readonly record struct Starts(HashSet<char> Chars, bool Nullable)
    {
        public static Starts Anything => new(null, false);
    }

    static readonly HashSet<char> _digits = [.. "0123456789"];
    static readonly string _regexSpecials = "()[].^$|?*+\\";

    static Starts Analyze(RegexNode node) =>
        node switch
        {
            // A wildcard capture, and a separator-led repetition (see JoinedItemNode): anything.
            DynamicGlyphNode or JoinedItemNode => Starts.Anything,
            TextNode text => FromRegexText(text.Text),
            TerminalRegexNode terminal => FromRegexText(terminal.RegexString),
            NamedGroupNode group => AnalyzeGroup(group),
            _ => Starts.Anything,
        };

    static Starts AnalyzeGroup(NamedGroupNode group)
    {
        var children = group.Children;

        // Alternatives - an enum's members, a one-of's properties, several patterns on one terminal - may each
        // begin the match. A one-of opening with literal text (see GlyphOneOfNode) is a sequence instead.
        bool isAlternation =
            (group.EffectiveChildJoiner == Joiner.Pipe && children.FirstOrDefault() is not TextNode)
            || children.All(x => x is TerminalRegexNode);

        var starts = isAlternation ? Union(children) : Sequence(group, children);

        return starts with { Nullable = starts.Nullable || group.IsNullable };
    }

    static Starts Union(List<RegexNode> children)
    {
        var chars = new HashSet<char>();
        bool nullable = children.Count == 0;

        foreach (var child in children)
        {
            var starts = Analyze(child);

            if (starts.Chars is null)
                return Starts.Anything;

            chars.UnionWith(starts.Chars);
            nullable |= starts.Nullable;
        }

        return new(chars, nullable);
    }

    /// <summary>The children in order, each able to begin the match only while every one before it can match nothing.</summary>
    static Starts Sequence(NamedGroupNode group, List<RegexNode> children)
    {
        var chars = new HashSet<char>();

        for (int i = 0; i < children.Count; i++)
        {
            // A later child may carry the joiner in front of it (inside its own group, if it's nullable), so that
            // joiner can begin the match too. Adding it when it isn't actually emitted only widens the set.
            if (i > 0)
            {
                var joinerStart = JoinerStart(JoinerRules.Intended(JoinSite.Of(group, children[i])));

                if (joinerStart.Chars is null)
                    return Starts.Anything;

                chars.UnionWith(joinerStart.Chars);
            }

            var starts = Analyze(children[i]);

            if (starts.Chars is null)
                return Starts.Anything;

            chars.UnionWith(starts.Chars);

            if (!starts.Nullable)
                return new(chars, false);
        }

        return new(chars, true);
    }

    static Starts JoinerStart(Joiner joiner) =>
        joiner switch
        {
            Joiner.None => new([], true),
            Joiner.Space => new([' '], false),
            Joiner.CommaSpace => new([','], false),
            Joiner.Dash => new(['-'], false),
            Joiner.Underscore => new(['_'], false),
            _ => Starts.Anything, // Dot renders as "." - any character - and Pipe never joins a sequence
        };

    /// <summary>
    /// What a fragment of regex text can begin with, understanding just enough of it for the nibs and patterns
    /// authors actually write: a literal character, an escaped one (<c>\{</c>, <c>\$</c>, <c>\.</c>), <c>\d</c>, and
    /// an escaped space (<c>[ ]</c>). Anything else - a group, a class, a first atom made optional, a top-level
    /// alternation - is <see cref="Starts.Anything"/>.
    /// </summary>
    static Starts FromRegexText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return new([], true);

        if (HasTopLevelAlternation(text))
            return Starts.Anything;

        HashSet<char> first;
        int atomLength;

        if (text.StartsWith(BuiltRegex.EscapedSpace))
            (first, atomLength) = ([' '], BuiltRegex.EscapedSpace.Length);
        else if (text[0] == '\\' && text.Length > 1)
        {
            var escaped = text[1];

            if (escaped == 'd')
                first = _digits;
            else if (char.IsLetterOrDigit(escaped))
                return Starts.Anything; // \w, \s, \b and the like
            else
                first = [escaped];

            atomLength = 2;
        }
        else if (_regexSpecials.Contains(text[0]))
            return Starts.Anything;
        else
            (first, atomLength) = ([text[0]], 1);

        // A quantifier that allows zero of the first atom means the match can begin further in.
        if (atomLength < text.Length && text[atomLength] is '?' or '*' or '{')
            return Starts.Anything;

        return new(first, false);
    }

    static bool HasTopLevelAlternation(string text)
    {
        int depth = 0;
        bool inClass = false;

        for (int i = 0; i < text.Length; i++)
        {
            switch (text[i])
            {
                case '\\': i++; break;
                case '[': inClass = true; break;
                case ']': inClass = false; break;
                case '(' when !inClass: depth++; break;
                case ')' when !inClass: depth--; break;
                case '|' when !inClass && depth == 0: return true;
            }
        }

        return false;
    }
}
