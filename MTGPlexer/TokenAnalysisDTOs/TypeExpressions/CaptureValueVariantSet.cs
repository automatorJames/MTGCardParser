namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public class CaptureValueVariantSet
{
    public object CanonicalValue { get; }
    public string CanonicalValueDisplay { get; }
    public Dictionary<string, int> VariantCounts { get; }
    public int TotalCount => VariantCounts.Sum(x => x.Value);

    public CaptureValueVariantSet(object canonicalValue, Capture variantCapture)
    {
        CanonicalValue = canonicalValue;
        CanonicalValueDisplay = ToFriendlyStringOrPattern(canonicalValue);
        VariantCounts = [];
        IncrementVariantCapture(variantCapture);
    }

    public void IncrementVariantCapture(Capture variantCapture)
    {
        var stringValue = variantCapture.Value;
        VariantCounts.TryAdd(stringValue, 0);
        VariantCounts[stringValue]++;
    }

    public override string ToString()
    {
        var mainString = $"{CanonicalValue}: {TotalCount}";

        if (!VariantCounts.Select(x => x.Key).Any(x => x != CanonicalValue.ToString()))
            return mainString;

        var subPartStr = string.Join(" | ", VariantCounts.Select(x => $"{x.Key}: {x.Value}"));

        return $"{mainString} ({subPartStr})";
    }
}
