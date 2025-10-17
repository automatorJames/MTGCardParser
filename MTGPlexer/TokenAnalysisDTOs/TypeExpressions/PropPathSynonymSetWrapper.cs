namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public class PropPathSynonymSetWrapper
{
    public CaptureGroupPropPath ParentPath { get; }
    public int AlternateCount { get; }
    public Dictionary<object, CaptureValueSynonymSet> SynonymSets { get; private set; } = [];

    public PropPathSynonymSetWrapper(CaptureGroupPropPath parentPath, RegexPropInfo prop)
    {
        ParentPath = parentPath;
        AlternateCount = prop.RegexPropType == RegexPropType.Enum ? Enum.GetValues(prop.BaseType).Length : 0;
    }

    public void OrderByOccurrenceCount()
    {
        SynonymSets = SynonymSets
            .OrderByDescending(x => x.Value.TotalCount)
            .ToDictionary(x => x.Key, x => x.Value);
    }
}
