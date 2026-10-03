namespace Glyphotype.RegexGeneration.Graph;

/// <summary>What one authored pattern (see <see cref="Nodes.TextNode.AuthoredPattern"/>) matched, within a match: its regex, and where in the source text.</summary>
public sealed record PatternCapture(string Pattern, int Index, int Length);

/// <summary>
/// The compiled result of walking a <see cref="RegexNode"/> graph: the flat brick sequence, the
/// concatenated pattern, and the anchored <see cref="System.Text.RegularExpressions.Regex"/>es the engine
/// runs it with. Formatted/commented output is a separate concern — see <see cref="ToSmartRegex"/>.
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

    readonly List<RegexBrick> _regexBricks;

    /// <summary>The flat, unformatted brick sequence this regex was compiled from — the raw input to <see cref="RegexBrickFormattingPipeline.Format"/>.</summary>
    public List<RegexBrick> Bricks => _regexBricks;

    /// <summary>
    /// The pattern itself: the concatenated raw regex text of every brick. This is the grammar - what's shown
    /// for analysis - and deliberately carries none of the anchors the engine adds to run it (see
    /// <see cref="AnchoredRegex"/>/<see cref="ScopeFillingRegex"/>). Those change how fast a match is found,
    /// never what it captures, so showing them on every type would be noise.
    /// </summary>
    public string MinifiedRegex { get; }

    readonly Lazy<Regex> _anchoredRegex;
    readonly Lazy<Regex> _scopeFillingRegex;
    readonly Lazy<PatternRegex> _patternRegex;

    /// <summary>
    /// <see cref="MinifiedRegex"/> with every authored pattern inside a named group of its own, and those patterns
    /// in group order - null when the graph has none. Analysis-only, and never what the engine matches with:
    /// adding groups changes nothing about what matches, but <see cref="MinifiedRegex"/>'s own text and length are
    /// read elsewhere (e.g. to order top-level types).
    /// </summary>
    sealed record PatternRegex(Regex ScopeFilling, Regex Anchored, IReadOnlyList<string> Patterns);

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

    public BuiltRegex(List<RegexBrick> regexBricks)
    {
        _regexBricks = regexBricks;
        MinifiedRegex = string.Join("", _regexBricks.Select(x => x.Regex)).Replace(EscapedSpace, " ");

        // Each built on first use: a type may only ever be matched one way (with the whole-clause rule on,
        // nearly every top-level type only ever has to fill its scope).
        _anchoredRegex = new(() => Compile($@"\G({MinifiedRegex})"));
        _scopeFillingRegex = new(() => Compile($@"\G({MinifiedRegex})\z"));
        _patternRegex = new(BuildPatternRegex);
    }

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

        var match = patternRegex.ScopeFilling.Match(sourceText, index, length);

        // The engine may have matched against the whole text (so lookarounds saw past the span) rather than a window.
        if (!match.Success)
            match = patternRegex.Anchored.Match(sourceText, index);

        if (!match.Success || match.Index != index || match.Length != length)
            return null;

        return patternRegex.Patterns
            .SelectMany((pattern, i) => match.Groups[_patternGroupPrefix + i].Captures
                .Where(x => x.Length > 0)
                .Select(x => new PatternCapture(pattern, x.Index, x.Length)))
            .OrderBy(x => x.Index)
            .ToList();
    }

    PatternRegex BuildPatternRegex()
    {
        List<string> patterns = [];
        var text = new StringBuilder();

        foreach (var brick in _regexBricks)
        {
            if (brick.Parent is Nodes.TextNode { AuthoredPattern: not null } node && brick.Regex == node.Text)
            {
                text.Append(node.WithPatternGroup(_patternGroupPrefix + patterns.Count));
                patterns.Add(node.AuthoredPattern);
            }
            else
            {
                text.Append(brick.Regex);
            }
        }

        if (patterns.Count == 0)
            return null;

        var regex = text.ToString().Replace(EscapedSpace, " ");

        return new(Compile($@"\G({regex})\z"), Compile($@"\G({regex})"), patterns);
    }

    /// <summary>
    /// Interpreted, not <see cref="RegexOptions.Compiled"/>: compiling these patterns to IL - some are large
    /// alternations over a whole vocabulary - costs far more up front than it saves per pass (measured on a
    /// ~15k-document corpus: 10.4s first pass then 376ms compiled, vs 444ms then 417ms interpreted). A grammar is
    /// rarely matched more than a few passes' worth - and every what-if grammar built from a definition is new
    /// types, so would pay the compile again.
    /// </summary>
    static Regex Compile(string pattern) =>
        new(pattern, RegexOptions.ExplicitCapture);

    /// <summary>Builds the formatted, colorized, commented representation of this regex for human-readable output.</summary>
    public SmartRegex ToSmartRegex(GlyphOccurrenceSummary summary, RegexGraph regexGraph, bool includeSupplementalLines = true, RegexDisplayMode displayMode = RegexDisplayMode.MatchedOnly) =>
        new(_regexBricks, summary, regexGraph, includeSupplementalLines, displayMode);

    /// <summary>
    /// Builds a single unpadded, uncommented line of this regex's raw text (the same characters as
    /// <see cref="MinifiedRegex"/>), colored per named group like the formatted view but with no line
    /// breaks, box comments, or enum member ranking/filtering applied.
    /// </summary>
    public List<SmartLine> ToRichMinifiedLines(RegexGraph regexGraph) =>
        [SmartLineRenderer.RenderMinifiedLine(_regexBricks, regexGraph)];

    public override string ToString() => MinifiedRegex;
}
