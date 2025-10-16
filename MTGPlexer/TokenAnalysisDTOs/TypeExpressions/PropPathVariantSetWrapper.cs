namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public class PropPathVariantSetWrapper
{
    public CaptureGroupPropPath ParentPath { get; }
    public int AlternateCount { get; }
    public Dictionary<object, CaptureValueVariantSet> VariantSets { get; private set; } = [];

    public PropPathVariantSetWrapper(CaptureGroupPropPath parentPath, RegexPropInfo prop)
    {
        ParentPath = parentPath;
        AlternateCount = prop.RegexPropType == RegexPropType.Enum ? Enum.GetValues(prop.BaseType).Length : 0;
    }

    public void OrderByOccurrenceCount()
    {
        VariantSets = VariantSets
            .OrderByDescending(x => x.Value.TotalCount)
            .ToDictionary(x => x.Key, x => x.Value);
    }
}
