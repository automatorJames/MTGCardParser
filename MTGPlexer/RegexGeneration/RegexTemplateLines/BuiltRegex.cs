namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public class BuiltRegex
{
    int _spacesPerIndent = 4;
    List<RegexBrick> _regexBricks;

    public string MinifiedRegex { get; }
    public string FormattedRegex { get; }
    public List<string> FormattedLines { get; }
    public Regex Regex { get; }

    public BuiltRegex(List<RegexBrick> regexBricks)
    {
        _regexBricks = regexBricks;
        MinifiedRegex = string.Join("", _regexBricks.Select(x => x.Regex)).Replace("[ ]", " ");
        FormatNamedGroups(_regexBricks);
        FormattedLines = _regexBricks.Select(x => new string(' ', x.NestedDepth * _spacesPerIndent) + x.RegexFormatted).ToList();
        FormattedRegex = string.Join(Environment.NewLine, FormattedLines);
        Regex = new(MinifiedRegex, RegexOptions.Compiled | RegexOptions.ExplicitCapture);
    }

    void FormatNamedGroups(List<RegexBrick> regexBricks)
    {
        var groupOpenBricksPendingFormatting = regexBricks.OfType<RegexBrickGroupOpen>().ToList();
        int minimumDepthForDistinctName = 1;

        while (groupOpenBricksPendingFormatting.Any())
        {
            var nameGroups = groupOpenBricksPendingFormatting.ToList().GroupBy(x => string.Join("_", x.GroupLineageNames.Take(minimumDepthForDistinctName)));

            foreach (var group in nameGroups.Where(x => x.Count() == 1))
            {
                var singularlyNamedBrick = group.First();
                singularlyNamedBrick.SetFormattedGroupName(group.Key);
                groupOpenBricksPendingFormatting.Remove(singularlyNamedBrick);
            }

            minimumDepthForDistinctName++;
        }
    }

    //public SmartRegex ToSmartRegex(TokenOccurrenceSummary summary, RegexGraph regexGraph)
    //{
    //    var namedGroupPalettes = DeterministicPalette.GetPositionalPaletteSet(regexGraph.NamedGroupFlatGraph.Keys);
    //
    //    Dictionary<EnumNode, List<RegexBrick>> enumBricks = regexGraph.NamedGroupFlatGraph.Values
    //        .OfType<EnumNode>()
    //        .ToDictionary(x => x, 
    //            x => _regexBricks
    //            .Where(y => y.NamedGroupParent.FullyQualifiedName == x.FullyQualifiedName)
    //            .OfType<INamedScalarValue>()
    //            .OrderByDescending(y => summary.EnumCaptureSummaries[y.NamedGroupParent.FullyQualifiedName].)
    //            .ToList());
    //
    //    var nonEnumRegexBricks = _regexBricks.Except(enumBricks.SelectMany(x => x.Value));
    //    
    //    foreach (var brick in _regexBricks)
    //    {
    //        var regex = brick.Regex;
    //        var palette = namedGroupPalettes[brick.NamedGroupParent.FullyQualifiedName];
    //
    //        if (brick is RegexBrickGroupOpen)
    //        {
    //            regex.Replace(brick.FullyQualifiedName, regexGraph.SimpleUniqueNames[brick.FullyQualifiedName]);
    //
    //            if (brick.NamedGroupParent is EnumNode)
    //            {
    //
    //            }
    //        }
    //
    //        // group/order/replace/hide enum entries
    //    }
    //}

    public override string ToString() => MinifiedRegex;
}
