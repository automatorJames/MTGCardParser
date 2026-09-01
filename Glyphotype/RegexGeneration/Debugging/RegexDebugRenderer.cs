namespace Glyphotype.RegexGeneration.Debugging;

/// <summary>
/// Presentation bridge for <see cref="RegexDebugResult"/>: renders the analyzed graph (and its
/// max-matching stem) through the exact same formatting pipeline the Glyph Regex page uses, and resolves a
/// result's <see cref="RegexDebugResult.FirstFailureBrick"/> to its line index within that rendering.
/// Lives here (rather than in the UI project) because the formatting pipeline is internal to this assembly.
/// </summary>
public static class RegexDebugRenderer
{
    /// <summary>
    /// Renders the full original graph — formatted identically to the Glyph Regex page's "Full" display
    /// mode — and returns the 0-based line index of the first failure (-1 for a full match), computed
    /// against this same formatted output so it agrees with what's on screen.
    /// </summary>
    public static (List<SmartLine> Lines, int FailureLineIndex) RenderOriginal(RegexDebugResult result, GlyphOccurrenceSummary summary)
    {
        var formatted = Format(result.Graph, summary, result.Graph.BuiltRegex.Bricks);
        var lines = SmartLineRenderer.Render(formatted, result.Graph);
        return (lines, GetFailureLineIndex(result, formatted));
    }

    /// <summary>Renders the result's max-matching stemmed permutation through the same pipeline, so it reads exactly like a (shorter) Glyph Regex page rendering of the type.</summary>
    public static List<SmartLine> RenderMaxMatchStem(RegexDebugResult result, GlyphOccurrenceSummary summary)
    {
        var formatted = Format(result.Graph, summary, result.MaxMatchStemBricks);
        return SmartLineRenderer.Render(formatted, result.Graph);
    }

    /// <summary>The 0-based formatted line index of <paramref name="result"/>'s first failure, without rendering — for the results table's "Failed Line #" column.</summary>
    public static int GetFailureLineIndex(RegexDebugResult result, GlyphOccurrenceSummary summary)
    {
        if (result.FirstFailureBrick == null)
            return -1;

        return GetFailureLineIndex(result, Format(result.Graph, summary, result.Graph.BuiltRegex.Bricks));
    }

    static List<RegexBrick> Format(RegexGraph graph, GlyphOccurrenceSummary summary, List<RegexBrick> bricks)
    {
        var pipeline = new RegexBrickFormattingPipeline(graph, summary, RegexDisplayMode.Full);
        return pipeline.Format(bricks, includeSupplementalLines: true);
    }

    /// <summary>
    /// <see cref="SmartLineRenderer.Render"/> emits exactly one line per formatted brick, so a brick's
    /// index in the formatted sequence is its line index. The failure brick itself usually survives
    /// formatting (a failing group's open bookend always does); when it doesn't (e.g. a joiner folded onto
    /// the preceding literal's line), walk backward through the raw sequence to the nearest brick that did.
    /// </summary>
    static int GetFailureLineIndex(RegexDebugResult result, List<RegexBrick> formattedBricks)
    {
        if (result.FirstFailureBrick == null)
            return -1;

        var rawBricks = result.Graph.BuiltRegex.Bricks;
        int rawIdx = rawBricks.IndexOf(result.FirstFailureBrick);

        for (int i = rawIdx; i >= 0; i--)
        {
            int lineIdx = formattedBricks.IndexOf(rawBricks[i]);

            if (lineIdx >= 0)
                return lineIdx;
        }

        return -1;
    }
}
