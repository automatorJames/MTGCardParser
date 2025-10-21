namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public class PropPathSynonymSetContainer
{
    public CaptureGroupPropPath ParentPath { get; }
    public int AlternateCount { get; }
    public Dictionary<object, CaptureValueSynonymSet> SynonymSets { get; private set; } = [];

    public PropPathSynonymSetContainer(CaptureGroupPropPath parentPath, RegexPropInfo prop = null)
    {
        ParentPath = parentPath;

        AlternateCount = prop == null || prop.RegexPropType != RegexPropType.Enum
            ? 0
            : Enum.GetValues(prop.BaseType).Length;
    }

    public void OrderByOccurrenceCount()
    {
        SynonymSets = SynonymSets
            .OrderByDescending(x => x.Value.TotalCount)
            .ToDictionary(x => x.Key, x => x.Value);
    }
}
