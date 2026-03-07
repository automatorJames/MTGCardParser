using MTGPlexer.RegexGeneration.Graph.Bricks;

namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

/// <summary>
/// Manages the construction of a logical sequence of regular expression elements. Acts as the single interface to translate RegexSegmentBase
/// components into properly-concatenated RegexElements, and ultimately composed Regex patterns. 
/// </summary>
public class RegexCollector
{
    public List<RegexBrick> RegexBricks { get; } = [];

    public char LastChar =>
        RegexBricks.LastOrDefault(x => x is not RegexBrickGroupBookend)
        .Regex.LastOrDefault();

    public void Append(RegexBrick brick) =>
        RegexBricks.Add(brick);

    public void AppendJoined(List<RegexNode> nodes, RegexBrick joiner)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].AppendRegexBricks(this);

            if (i < nodes.Count - 1)
                Append(joiner);
        }
    }

    public void AppendJoinedAlternating(RegexNode parentNode, List<RegexNode> childNodesToJoin)
    {
        for (int i = 0; i < childNodesToJoin.Count; i++)
        {
            childNodesToJoin[i].AppendRegexBricks(this);

            if (i < childNodesToJoin.Count - 1)
                Append(new RegexBrickJoiner(parentNode, Joiner.Pipe));
        }
    }


    ///// <summary>
    ///// Generates a fully formatted, commented, and colorized list of regex lines.
    ///// </summary>
    ///// <param name="synonymData">Optional data about captured synonyms to enrich the comments.</param>
    ///// <returns>A list of formatted regex lines.</returns>
    //public List<RegexFormattedLine> GetFormattedLines(List<PropPathSynonymSetContainer> synonymData = null)
    //{
    //    //var finalizedElements = _joiner.RegexElements.ToList();
    //    //AddBoundaryLines(finalizedElements);
    //    //var formatter = new RegexFormatter(finalizedElements, synonymData);
    //    //return formatter.Format();
    //
    //    return default;
    //}

    public BuiltRegex GetBuiltRegex() => new(RegexBricks);

    public override string ToString() => 
        string.Join("", RegexBricks.Select(x => x.Regex));
}