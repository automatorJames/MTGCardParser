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

    public override string ToString() => $"{ParentPath}: {(SynonymSets.Count == 1 ? SynonymSets.First().Value.CanonicalValueDisplay : SynonymSets.Count + " synonym sets")}";
}
