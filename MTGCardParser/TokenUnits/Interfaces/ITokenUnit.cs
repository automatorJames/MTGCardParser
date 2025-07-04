namespace MTGCardParser.TokenUnits.Interfaces;

public interface ITokenUnit
{
    public RegexTemplate GetRegexTemplate();
    public void SetPropertiesFromMatchSpan();
    public ITokenUnit ParentToken { get; set; }
    public List<ITokenUnit> ChildTokens { get; set; }
    public int RecursiveDepth { get; set; }
    public TextSpan MatchSpan { get; set; }
    public Dictionary<CaptureProp, TextSpan> PropMatches { get; set; }

    public int GetDeepestChildLevel()
    {
        IEnumerable<ITokenUnit> childrenAtCurrentRecursiveLevel = ChildTokens;
        var deepestDepth = 0;

        while (childrenAtCurrentRecursiveLevel.Any())
        {
            deepestDepth++;
            childrenAtCurrentRecursiveLevel = childrenAtCurrentRecursiveLevel.SelectMany(x => x.ChildTokens);
        }

        return deepestDepth;
    }

    public NestedTokenSpan GetNestedSpan()
    {
        var flatGraph = ChildTokens
            .SelectMany(x => x.ChildTokens)
            .Concat([this])
            .OrderBy(x => x.MatchSpan.Position.Absolute)
            .ToList();

        
    }
}

