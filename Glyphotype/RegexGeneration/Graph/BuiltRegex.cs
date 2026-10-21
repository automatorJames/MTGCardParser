using System.Runtime.CompilerServices;

namespace Glyphotype.RegexGeneration.Graph;

/// <summary>What one authored pattern (see <see cref="Nodes.TextNode.AuthoredPattern"/>) matched, within a match: its regex, and where in the source text.</summary>
public sealed record PatternCapture(string Pattern, int Index, int Length);

/// <summary>
/// The compiled result of walking a <see cref="RegexNode"/> graph: the concatenated pattern, the anchored
/// <see cref="System.Text.RegularExpressions.Regex"/>es the engine runs it with, and - built again whenever they're
/// asked for and nothing holds the last ones (see <see cref="Bricks"/>) - the flat brick sequence the formatted display
/// is made from. Formatted/commented output is a separate concern — see <see cref="ToSmartRegex"/>.
/// </summary>
public class BuiltRegex
{
    /// <summary>
    /// The token a literal space is written as throughout the graph's brick text (see
    /// <see cref="Nodes.TextNode"/>) - the same text <see cref="Joiner.Space"/> itself renders as, so a
    /// space that came from a nib and one that came from a joiner are indistinguishable downstream. Every
    /// occurrence is unescaped back to a plain space in <see cref="MinifiedRegex"/> before compiling, so
    /// this is currently a display/authoring convention only; escaping it here is what would let
    /// <see cref="RegexOptions.IgnorePatternWhitespace"/> be turned on later (drop the unescape, add the
    /// flag) without every literal space silently vanishing from the pattern.
    /// </summary>
    public static readonly string EscapedSpace = Joiner.Space.GetDescription();

    /// <summary>
    /// Rewrites every literal space in <paramref name="regexText"/> as <see cref="EscapedSpace"/>. Idempotent
    /// (it unescapes first), so text that already spells some or all of its spaces that way lands on exactly
    /// the same result as text written with plain spaces - which is what lets a nib authored <c>","</c>,
    /// <c>", "</c> and <c>",[ ]"</c> all come out identically.
    /// </summary>
    public static string EscapeSpaces(string regexText) =>
        regexText?.Replace(EscapedSpace, " ").Replace(" ", EscapedSpace);

    readonly GlyphNode _root;
    readonly object _bricksGate = new();

    /// <summary>The bricks last built, held only while something else holds them too - see <see cref="Bricks"/>.</summary>
    WeakReference<List<RegexBrick>> _bricks;

    /// <summary><see cref="IndexMembersByEnclosingEnum"/> of each brick list built, for as long as that list lives.</summary>
    readonly ConditionalWeakTable<List<RegexBrick>, ILookup<string, RegexBrickValue>> _membersByEnclosingEnum = new();

    /// <summary>
    /// The flat, unformatted brick sequence this regex was compiled from — the raw input to <see cref="RegexBrickFormattingPipeline.Format"/>.
    /// Not kept: a large grammar's graphs produce millions of bricks, nearly all of them vocabulary members, that only
    /// the formatted display ever reads. The sequence is built again from the graph when it's asked for and the last
    /// one built has been let go of; while something holds it, the same list is handed out, so a caller formatting it
    /// finds the same objects it was given.
    /// </summary>
    public List<RegexBrick> Bricks
    {
        get
        {
            lock (_bricksGate)
            {
                if (_bricks is not null && _bricks.TryGetTarget(out var bricks))
                    return bricks;

                bricks = Walk(_root);
                _bricks = new(bricks);
                return bricks;
            }
        }
    }

    /// <summary>
    /// The pattern itself: the concatenated raw regex text of every brick. This is the grammar - what's shown
    /// for analysis - and deliberately carries none of the anchors the engine adds to run it (see
    /// <see cref="AnchoredRegex"/>/<see cref="ScopeFillingRegex"/>). Those change how fast a match is found,
    /// never what it captures, so showing them on every type would be noise.
    /// </summary>
    public string MinifiedRegex { get; }

    /// <summary>
    /// The pattern as the engine matches it (see <see cref="Matching.Matcher"/>): <see cref="MinifiedRegex"/> cut into runs the
    /// regex engine runs as they are, and calls to the plans of the large nested glyphs - shared with every other type
    /// nesting them - so the flattened regex itself is never compiled for matching, only kept to be shown.
    /// </summary>
    public Matching.MatchPlan Plan { get; }

    readonly Lazy<Regex> _anchoredRegex;
    readonly Lazy<Regex> _scopeFillingRegex;
    readonly Lazy<PatternRegex> _patternRegex;

    /// <summary>
    /// <see cref="MinifiedRegex"/> with every authored pattern inside a named group of its own, and those patterns
    /// in group order - null when the graph has none. Analysis-only, and never what the engine matches with:
    /// adding groups changes nothing about what matches, but <see cref="MinifiedRegex"/>'s own text and length are
    /// read elsewhere (e.g. to order top-level types). Its other named groups are plain groups, which change nothing
    /// about what matches either (no pattern refers back to a group): a match allocates for every group its regex
    /// has, and the largest have tens of thousands, of which only the pattern groups are read.
    /// </summary>
    /// <param name="Anchored">Compiled on first need: it's the fallback for a span the scope-filling regex can't match again, which is rare.</param>
    sealed record PatternRegex(Regex ScopeFilling, Lazy<Regex> Anchored, IReadOnlyList<string> Patterns);

    const string _patternGroupPrefix = "__pattern";

    /// <summary>
    /// <see cref="MinifiedRegex"/> anchored (<c>\G</c>) to the position it's run from - the window's start, or
    /// <c>startat</c>. A match is only ever wanted right there, so without the anchor a failed attempt would
    /// go on searching every later position to the window's end, for a match that would be thrown away.
    /// Execution-only; see <see cref="MinifiedRegex"/>.
    /// </summary>
    internal Regex AnchoredRegex => _anchoredRegex.Value;

    /// <summary>
    /// <see cref="AnchoredRegex"/>, also anchored (<c>\z</c>) to the end of the window it's run against - for a
    /// match that must fill its scope. Bounding the window alone doesn't make a match fill it: the regex engine
    /// settles on the first successful alternative, not the longest, so given "fish sticks" an alternation
    /// <c>fish|fish sticks</c> would stop at "fish", fall short of the window, and be rejected with the longer
    /// alternative never tried. The anchor makes falling short a failure the engine backtracks out of instead.
    /// Execution-only; see <see cref="MinifiedRegex"/>.
    /// </summary>
    internal Regex ScopeFillingRegex => _scopeFillingRegex.Value;

    /// <summary>Walks <paramref name="root"/>'s graph for its pattern, and sets up its regexes to compile on first use.</summary>
    public BuiltRegex(GlyphNode root)
    {
        _root = root;

        var bricks = Walk(root);
        MinifiedRegex = Share(string.Concat(bricks.Select(x => x.Regex)).Replace(EscapedSpace, " "));
        (_patternRegexText, _patterns) = BuildPatternText(bricks);
        TypedGroups = FindTypedGroups(bricks, MinifiedRegex.Length);
        Plan = Matching.MatchPlan.Build(root, bricks);
        _bricks = new(bricks);

        // Each built on first use: a type may only ever be matched one way (with the whole-clause rule on,
        // nearly every top-level type only ever has to fill its scope).
        _anchoredRegex = new(() => Compile($@"\G({MinifiedRegex})"));
        _scopeFillingRegex = new(() => Compile($@"\G({MinifiedRegex})\z"));
        _patternRegex = new(() => _patternRegexText is null ? null : new(Compile($@"\G({_patternRegexText})\z"), new(() => Compile($@"\G({_patternRegexText})")), _patterns));
    }

    /// <summary>The text <see cref="_patternRegex"/> compiles (see <see cref="PatternRegex"/>), or null for a graph with no authored patterns - found from the bricks while they're in hand, since building them again costs more than keeping the text.</summary>
    readonly string _patternRegexText;
    readonly IReadOnlyList<string> _patterns;

    /// <summary>
    /// Where each group capturing a glyph or a vocabulary sits in <see cref="MinifiedRegex"/>, in the order the groups
    /// open - found from the bricks while they're in hand, as <see cref="_patternRegexText"/> is. What tells whether
    /// two regexes differ only inside the groups of some types (see <see cref="Tokenizers.TokenizationChange"/>).
    /// Null if the bricks' text doesn't add up to <see cref="MinifiedRegex"/>'s.
    /// </summary>
    internal IReadOnlyList<TypedGroup> TypedGroups { get; }

    /// <summary>A group capturing a glyph or a vocabulary (<paramref name="TypeName"/>): its content, between its bookends, as a range of <see cref="MinifiedRegex"/>.</summary>
    internal readonly record struct TypedGroup(string TypeName, int ContentStart, int ContentEnd);

    static TypedGroup[] FindTypedGroups(List<RegexBrick> bricks, int textLength)
    {
        List<TypedGroup> groups = [];
        Stack<(RegexNode Node, int Index, int ContentStart)> open = [];
        var position = 0;

        foreach (var brick in bricks)
        {
            if (brick is RegexBrickGroupClose && open.TryPeek(out var top) && top.Node == brick.Parent)
            {
                open.Pop();
                groups[top.Index] = groups[top.Index] with { ContentEnd = position };
            }

            position += UnescapedLength(brick.Regex);

            if (brick is RegexBrickGroupOpen && brick.Parent is NamedGroupNode { Navigation.NodeType: { IsGenericType: false } type } node
                && (node.Navigation.IsGlyphType || type.IsEnum))
            {
                open.Push((node, groups.Count, position));
                groups.Add(new(type.Name, position, -1));
            }
        }

        return position == textLength && open.Count == 0 ? [.. groups] : null;
    }

    /// <summary>The length of <paramref name="regex"/> once its <see cref="EscapedSpace"/>s are plain spaces, as in <see cref="MinifiedRegex"/>.</summary>
    static int UnescapedLength(string regex)
    {
        var length = regex.Length;

        for (var index = regex.IndexOf(EscapedSpace, StringComparison.Ordinal); index >= 0; index = regex.IndexOf(EscapedSpace, index + EscapedSpace.Length, StringComparison.Ordinal))
            length -= EscapedSpace.Length - 1;

        return length;
    }

    /// <summary>
    /// <paramref name="text"/>, or the identical text another regex already holds: a grammar built from a definition
    /// emits a fresh type for every glyph that refers to a changed one, and most of their patterns come out character
    /// for character the same - at megabytes apiece for the largest. Held weakly, so a text no live regex has goes.
    /// </summary>
    static string Share(string text)
    {
        lock (_sharedTexts)
        {
            if (_sharedTexts.TryGetValue(text, out var reference) && reference.TryGetTarget(out var shared))
                return shared;

            if (++_sharedSinceSweep > 256)
            {
                foreach (var key in _sharedTexts.Where(x => !x.Value.TryGetTarget(out _)).Select(x => x.Key).ToList())
                    _sharedTexts.Remove(key);

                _sharedSinceSweep = 0;
            }

            _sharedTexts[text] = new(text);
            return text;
        }
    }

    static readonly Dictionary<string, WeakReference<string>> _sharedTexts = [];
    static int _sharedSinceSweep;

    /// <summary>The brick sequence <paramref name="root"/>'s graph produces.</summary>
    static List<RegexBrick> Walk(GlyphNode root)
    {
        RegexCollector collector = new();
        root.AppendRegexBricks(collector);
        return collector.RegexBricks;
    }

    /// <summary>Compiles the matching regexes now rather than on first match - for measuring, or for taking the cost before serving.</summary>
    public void WarmUp()
    {
        _ = _anchoredRegex.Value;
        _ = _scopeFillingRegex.Value;
    }

    /// <summary>
    /// <see cref="IndexMembersByEnclosingEnum"/> over <see cref="Bricks"/>, built once per brick list: a type embedded in
    /// others' dynamic groups is formatted again for every one, every render.
    /// </summary>
    public ILookup<string, RegexBrickValue> MembersByEnclosingEnum =>
        _membersByEnclosingEnum.GetValue(Bricks, IndexMembersByEnclosingEnum);

    /// <summary>
    /// Every enum member value brick in <paramref name="bricks"/>, keyed by the fully qualified name of each
    /// enum group it sits inside, in brick order - so formatting finds an enum's members by looking them up
    /// rather than scanning every brick once per enum.
    /// </summary>
    public static ILookup<string, RegexBrickValue> IndexMembersByEnclosingEnum(IEnumerable<RegexBrick> bricks) =>
        bricks
            .OfType<RegexBrickValue>()
            .SelectMany(member => member.NamedGroupNodeLineage.OfType<EnumNode>().Select(enumNode => (enumNode.FullyQualifiedName, member)))
            .ToLookup(x => x.FullyQualifiedName, x => x.member);

    /// <summary>Whether any part of this regex is a pattern an author wrote, rather than literal text or structure (see <see cref="Nodes.TextNode.AuthoredPattern"/>).</summary>
    public bool HasAuthoredPatterns => _patternRegex.Value is not null;

    /// <summary>
    /// What each authored pattern matched, in a match of this regex spanning exactly <paramref name="length"/>
    /// characters of <paramref name="sourceText"/> from <paramref name="index"/> - found by matching that span
    /// again with each pattern in a group of its own. The same regex over the same span backtracks the same way,
    /// so the groups report the very match the engine made. Empty-length captures (an optional pattern that
    /// matched nothing) are left out; null if the span can't be matched again.
    /// </summary>
    public IReadOnlyList<PatternCapture> FindPatternCaptures(string sourceText, int index, int length)
    {
        if (_patternRegex.Value is not PatternRegex patternRegex)
            return [];

        // One thread at a time per Regex, as RegexGraph.TryMatchRegex: see there.
        Match match;

        lock (patternRegex.ScopeFilling)
            match = patternRegex.ScopeFilling.Match(sourceText, index, length);

        // The engine may have matched against the whole text (so lookarounds saw past the span) rather than a window.
        if (!match.Success)
        {
            var anchored = patternRegex.Anchored.Value;

            lock (anchored)
                match = anchored.Match(sourceText, index);
        }

        if (!match.Success || match.Index != index || match.Length != length)
            return null;

        return patternRegex.Patterns
            .SelectMany((pattern, i) => match.Groups[_patternGroupPrefix + i].Captures
                .Where(x => x.Length > 0)
                .Select(x => new PatternCapture(pattern, x.Index, x.Length)))
            .OrderBy(x => x.Index)
            .ToList();
    }

    /// <summary>The text of <see cref="PatternRegex"/> and its patterns, from <paramref name="bricks"/> - or null and null, for a graph with no authored patterns.</summary>
    static (string Text, IReadOnlyList<string> Patterns) BuildPatternText(List<RegexBrick> bricks)
    {
        List<string> patterns = [];
        var text = new StringBuilder();

        foreach (var brick in bricks)
        {
            if (brick.Parent is Nodes.TextNode { AuthoredPattern: not null } node && brick.Regex == node.Text)
            {
                text.Append(node.WithPatternGroup(_patternGroupPrefix + patterns.Count));
                patterns.Add(node.AuthoredPattern);
            }
            else if (brick is Bricks.RegexBrickGroupOpen)
            {
                text.Append("(?:");
            }
            else
            {
                text.Append(brick.Regex);
            }
        }

        return patterns.Count == 0 ? (null, null) : (Share(text.ToString().Replace(EscapedSpace, " ")), patterns);
    }

    /// <summary>
    /// Interpreted, not <see cref="RegexOptions.Compiled"/>: compiling these patterns to IL - some are large
    /// alternations over a whole vocabulary - costs far more up front than it saves per pass (measured on a
    /// ~15k-document corpus: 10.4s first pass then 376ms compiled, vs 444ms then 417ms interpreted). A grammar is
    /// rarely matched more than a few passes' worth - and every what-if grammar built from a definition is new
    /// types, so would pay the compile again.
    /// </summary>
    internal static Regex Compile(string pattern)
    {
        // Shared by pattern text: a grammar built from a definition emits a fresh type - with a graph and regexes of its own - for
        // every glyph that so much as refers to a changed one, though most of their patterns come out character for character the
        // same. Held weakly, so a pattern no live graph has goes with its last graph.
        lock (_compiled)
        {
            if (_compiled.TryGetValue(pattern, out var reference) && reference.TryGetTarget(out var regex))
                return regex;

            if (++_compiledSinceSweep > 256)
            {
                foreach (var (key, dead) in _compiled.Where(x => !x.Value.TryGetTarget(out _)).ToList())
                    _compiled.Remove(key);

                _compiledSinceSweep = 0;
            }

            regex = new(pattern, RegexOptions.ExplicitCapture);
            _compiled[pattern] = new(regex);
            return regex;
        }
    }

    static readonly Dictionary<string, WeakReference<Regex>> _compiled = [];
    static int _compiledSinceSweep;

    /// <summary>Builds the formatted, colorized, commented representation of this regex for human-readable output.</summary>
    public SmartRegex ToSmartRegex(GlyphOccurrenceSummary summary, RegexGraph regexGraph, bool includeSupplementalLines = true, RegexDisplayMode displayMode = RegexDisplayMode.MatchedOnly, GroupNameStyle groupNameStyle = GroupNameStyle.ShortestUnique) =>
        new(Bricks, summary, regexGraph, includeSupplementalLines, displayMode, groupNameStyle);

    /// <summary>
    /// Builds a single unpadded, uncommented line of this regex's raw text (the same characters as
    /// <see cref="MinifiedRegex"/>), colored per named group like the formatted view but with no line
    /// breaks, box comments, or enum member ranking/filtering applied.
    /// </summary>
    public List<SmartLine> ToRichMinifiedLines(RegexGraph regexGraph) =>
        [SmartLineRenderer.RenderMinifiedLine(Bricks, regexGraph)];

    public override string ToString() => MinifiedRegex;
}
