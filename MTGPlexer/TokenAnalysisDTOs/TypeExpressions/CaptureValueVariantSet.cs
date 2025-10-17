namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public class CaptureValueVariantSet
{
    public object CanonicalValue { get; }
    public string CanonicalValueDisplay { get; }
    public Dictionary<string, int> SynonymCounts { get; }
    public int TotalCount => SynonymCounts.Sum(x => x.Value);

    public CaptureValueVariantSet(object canonicalValue, Capture variantCapture)
    {
        CanonicalValue = canonicalValue;
        CanonicalValueDisplay = ToFriendlyStringOrPattern(canonicalValue);
        SynonymCounts = [];
        IncrementVariantCapture(variantCapture);
    }

    public void IncrementVariantCapture(Capture variantCapture)
    {
        var stringValue = variantCapture.Value;
        SynonymCounts.TryAdd(stringValue, 0);
        SynonymCounts[stringValue]++;
    }

    public override string ToString()
    {
        var mainString = $"{CanonicalValue}: {TotalCount}";

        if (!SynonymCounts.Select(x => x.Key).Any(x => x != CanonicalValue.ToString()))
            return mainString;

        var subPartStr = string.Join(" | ", SynonymCounts.Select(x => $"{x.Key}: {x.Value}"));

        return $"{mainString} ({subPartStr})";
    }
}
