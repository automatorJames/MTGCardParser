namespace Glyphotype.RegexGeneration.Graph;

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

        // Each built on first use: a type may only ever be matched one way (with the whole-segment rule on,
        // nearly every top-level type only ever has to fill its scope).
        _anchoredRegex = new(() => Compile($@"\G({MinifiedRegex})"));
        _scopeFillingRegex = new(() => Compile($@"\G({MinifiedRegex})\z"));
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
