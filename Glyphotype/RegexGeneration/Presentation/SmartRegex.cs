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

    public SmartRegex(List<RegexBrick> bricks, GlyphOccurrenceSummary summary, RegexGraph regexGraph, bool includeSupplementalLines = true, RegexDisplayMode displayMode = RegexDisplayMode.MatchedOnly, GroupNameStyle groupNameStyle = GroupNameStyle.ShortestUnique)
    {
        var pipeline = new RegexBrickFormattingPipeline(regexGraph, summary, displayMode);
        var formattedBricks = pipeline.Format(bricks, includeSupplementalLines);
        NameGroups(formattedBricks, groupNameStyle);
        Lines = SmartLineRenderer.Render(formattedBricks, regexGraph);
    }

    /// <summary>
    /// Renames every group <paramref name="bricks"/> opens per <paramref name="style"/> - <see cref="GroupNameStyle.ShortestUnique"/>
    /// being what formatting already named them. Done over the finished sequence rather than by each formatting pass,
    /// so a type embedded in a dynamic group is numbered in with the groups around it.
    /// </summary>
    static void NameGroups(List<RegexBrick> bricks, GroupNameStyle style)
    {
        if (style == GroupNameStyle.ShortestUnique)
            return;

        var position = 0;

        foreach (var groupOpen in bricks.OfType<RegexBrickGroupOpen>())
        {
            var ownName = groupOpen.GroupName.Split('_')[^1];
            groupOpen.SetFormattedGroupName(style == GroupNameStyle.Numbered ? $"_{++position}_{ownName}" : ownName);
        }
    }

    public override string ToString() =>
        string.Join("\r\n", Lines);
}
