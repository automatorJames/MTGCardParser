namespace Glyphotype.RegexGeneration.Graph;

/// <summary>
/// The compiled result of walking a <see cref="RegexNode"/> graph: the flat brick sequence, the
/// concatenated matching pattern, and the compiled <see cref="System.Text.RegularExpressions.Regex"/>.
/// Formatted/commented output is a separate concern — see <see cref="ToSmartRegex"/>.
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

    /// <summary>The concatenated raw regex text of every brick, used to compile <see cref="Regex"/>.</summary>
    public string MinifiedRegex { get; }

    /// <summary>The compiled matching pattern.</summary>
    public Regex Regex { get; }

    public BuiltRegex(List<RegexBrick> regexBricks)
    {
        _regexBricks = regexBricks;
        MinifiedRegex = string.Join("", _regexBricks.Select(x => x.Regex)).Replace(EscapedSpace, " ");
        Regex = new(MinifiedRegex, RegexOptions.Compiled | RegexOptions.ExplicitCapture);
    }

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
