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
        FormatNamedGroups(regexBricks);
        FormattedLines = regexBricks.Where(x => !x.Parent.MayIgnoreInFormattedOutput).Select(x => new string(' ', x.NestedDepthFormatted * 4) + x.RegexFormatted).ToList();
        FormattedRegex = string.Join(Environment.NewLine, FormattedLines);
        Regex = new(MinifiedRegex, RegexOptions.Compiled | RegexOptions.ExplicitCapture);
    }

    void FormatNamedGroups(List<RegexBrick> regexBricks)
    {
        var groupOpenBricksPendingFormatting = regexBricks.OfType<RegexBrickGroupOpen>().ToList();
        int minimumDepthForDistinctName = 1;

        while (groupOpenBricksPendingFormatting.Any())
        {
            var nameGroups = groupOpenBricksPendingFormatting.ToList().GroupBy(x => string.Join("_", x.ContainingGroupNames.Take(minimumDepthForDistinctName)));

            foreach (var group in nameGroups.Where(x => x.Count() == 1))
            {
                var singularlyNamedBrick = group.First();
                singularlyNamedBrick.SetFormattedGroupName(group.Key);
                groupOpenBricksPendingFormatting.Remove(singularlyNamedBrick);
            }

            minimumDepthForDistinctName++;
        }
    }

    public override string ToString() => MinifiedRegex;
}
