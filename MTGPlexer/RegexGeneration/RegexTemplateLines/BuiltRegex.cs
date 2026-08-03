namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public class BuiltRegex
{
    List<RegexBrick> _regexBricks;

    public string MinifiedRegex { get; }
    //public string FormattedRegex { get; }
    //public List<string> FormattedLines { get; }
    public Regex Regex { get; }

    public BuiltRegex(List<RegexBrick> regexBricks)
    {
        _regexBricks = regexBricks;
        MinifiedRegex = string.Join("", _regexBricks.Select(x => x.Regex)).Replace("[ ]", " ");
        //FormatNamedGroups(_regexBricks);
        //FormattedLines = _regexBricks.Select(x => new string(' ', x.NestedDepth * SmartRegexStaticRules.SpacesPerIndent) + x.RegexFormatted).ToList();
        //FormattedRegex = string.Join(Environment.NewLine, FormattedLines);
        Regex = new(MinifiedRegex, RegexOptions.Compiled | RegexOptions.ExplicitCapture);
    }

    //void FormatNamedGroups(List<RegexBrick> regexBricks)
    //{
    //    var groupOpenBricksPendingFormatting = regexBricks.OfType<RegexBrickGroupOpen>().ToList();
    //    int minimumDepthForDistinctName = 1;
    //
    //    while (groupOpenBricksPendingFormatting.Any())
    //    {
    //        var nameGroups = groupOpenBricksPendingFormatting.ToList().GroupBy(x => string.Join("_", x.GroupLineageNames.Take(minimumDepthForDistinctName)));
    //
    //        foreach (var group in nameGroups.Where(x => x.Count() == 1))
    //        {
    //            var singularlyNamedBrick = group.First();
    //            singularlyNamedBrick.SetFormattedGroupName(group.Key);
    //            groupOpenBricksPendingFormatting.Remove(singularlyNamedBrick);
    //        }
    //
    //        minimumDepthForDistinctName++;
    //    }
    //}

    public SmartRegex ToSmartRegex(TokenOccurrenceSummary summary, RegexGraph regexGraph) =>
        new(_regexBricks, summary, regexGraph);

    public override string ToString() => MinifiedRegex;
}
