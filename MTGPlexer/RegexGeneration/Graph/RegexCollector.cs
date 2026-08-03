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

    public BuiltRegex GetBuiltRegex() => 
        new(RegexBricks);

    public override string ToString() => 
        string.Join("", RegexBricks.Select(x => x.Regex));
}