namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public class CaptureValueVariantSet
{
    public object CanonicalValue { get; }
    public object CanonicalValueDisplay { get; }
    public Dictionary<string, int> VariantCounts { get; }
    public RegexCommentedAlternateLine MatchingLine { get; }
    public int TotalCount => VariantCounts.Sum(x => x.Value);

    public CaptureValueVariantSet(RegexCommentedAlternateLine matchingLine, string variantValue)
    {
        CanonicalValue = matchingLine.CanonicalValue;
        CanonicalValueDisplay = matchingLine.CanonicalValueDisplay;
        MatchingLine = matchingLine;
        VariantCounts = new Dictionary<string, int>() { [variantValue] = 1 };
    }

    public void IncrementVariant(string variantValue)
    {
        VariantCounts.TryAdd(variantValue, 0);
        VariantCounts[variantValue]++;
    }

    public override string ToString()
    {
        var mainString = $"{CanonicalValue}: {TotalCount}";

        if (!VariantCounts.Select(x => x.Key).Any(x => x != CanonicalValue))
            return mainString;

        var subPartStr = string.Join(" | ", VariantCounts.Select(x => $"{x.Key}: {x.Value}"));

        return $"{mainString} ({subPartStr})";
    }
}
