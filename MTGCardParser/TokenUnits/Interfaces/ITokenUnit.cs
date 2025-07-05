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

    public List<ITokenUnit> FlattenTokenTree(ITokenUnit root = null)
    {
        root ??= this;
        var list = new List<ITokenUnit>();

        void Recurse(ITokenUnit token)
        {
            list.Add(token);
            if (token.ChildTokens != null)
            {
                foreach (var child in token.ChildTokens)
                    Recurse(child);
            }
        }

        Recurse(root);

        return list
            .OrderBy(t => t.MatchSpan.Position.Absolute)
            .ToList();
    }
}

