namespace Glyphotype.RegexGeneration.Presentation;

/// <summary>
/// The fully-formatted, colorized, line-by-line rendering of a compiled regex: each <see cref="SmartLine"/>
/// pairs a brick's (possibly re-formatted) regex text with a boxed comment describing what it matches,
/// using occurrence data from a <see cref="GlyphOccurrenceSummary"/> to rank and group enum members.
/// Formatting happens in two stages: <see cref="RegexBrickFormattingPipeline"/> decides what every brick
/// says and where blank lines go, then <see cref="SmartLineRenderer"/> lays that out as colored spans.
/// </summary>
public class SmartRegex
{
    /// <summary>The rendered lines, in display order.</summary>
    public List<SmartLine> Lines { get; }

    public SmartRegex(List<RegexBrick> bricks, GlyphOccurrenceSummary summary, RegexGraph regexGraph, bool includeSupplementalLines = true, RegexDisplayMode displayMode = RegexDisplayMode.MatchedOnly)
    {
        var pipeline = new RegexBrickFormattingPipeline(regexGraph, summary, displayMode);
        var formattedBricks = pipeline.Format(bricks, includeSupplementalLines);
        Lines = SmartLineRenderer.Render(formattedBricks, regexGraph);
    }

    public override string ToString() =>
        string.Join("\r\n", Lines);
}
