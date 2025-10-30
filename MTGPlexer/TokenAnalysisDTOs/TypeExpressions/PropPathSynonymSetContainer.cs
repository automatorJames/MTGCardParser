namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public class PropPathSynonymSetContainer
{
    public CaptureGroupPropPath ParentPath { get; }
    public int AlternateCount { get; }
    public int UnrepresentedAlternateCount => AlternateCount - SynonymSets.Count;
    public Dictionary<object, CaptureValueSynonymSet> SynonymSets { get; private set; } = [];

    public PropPathSynonymSetContainer(CaptureGroupPropPath parentPath, RegexPropInfo prop = null)
    {
        ParentPath = parentPath;

        //if (prop == null)
        //    AlternateCount = 0;
        //else if (prop.RegexPropType == RegexPropType.Enum)
        //    AlternateCount = Enum.GetValues(prop.BaseType).Length;
        //else if (prop.RegexPropType == RegexPropType.ManyOf && prop.BaseType.IsEnum)
        //    AlternateCount = Enum.GetValues(prop.BaseType).Length;

        AlternateCount = prop == null || !prop.BaseType.IsEnum
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
