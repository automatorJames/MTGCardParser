using MTGPlexer.RegexGeneration.Graph;

namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public class BuiltRegex
{
    public string MinifiedRegex { get; }
    public string FormattedRegex { get; }
    public List<string> FormattedLines { get; }
    public Regex Regex { get; }

    public BuiltRegex(List<RegexBrick> regexBricks)
    {
        MinifiedRegex = string.Join("", regexBricks.Select(x => x.Regex)).Replace("[ ]", " ");
        var bricksMinusOuterBookends = regexBricks.Skip(1).Take(regexBricks.Count - 2);
        FormattedLines = bricksMinusOuterBookends.Select(x => new string(' ', (x.NestedDepth - 1) * 4) + x.Regex).ToList();
        FormattedRegex = string.Join(Environment.NewLine, FormattedLines);
        Regex regex = new(MinifiedRegex, RegexOptions.Compiled | RegexOptions.ExplicitCapture);
    }

    public override string ToString() => MinifiedRegex;
}
