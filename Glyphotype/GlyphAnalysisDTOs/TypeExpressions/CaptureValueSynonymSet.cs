/*namespace Glyphotype.GlyphAnalysisDTOs.TypeExpressions;

public class CaptureValueSynonymSet
{
    public object CanonicalValue { get; }
    public string CanonicalValueDisplay { get; }
    public Dictionary<string, int> SynonymCounts { get; }
    public int TotalCount => SynonymCounts.Sum(x => x.Value);

    /// <summary>
    /// Space-separated set of paths used for ManyOf values so that each terminal value (first, secondPlus, last, and conjunction) 
    /// within the whole ManyOf value be related by data-path to its associated formatted Regex line.
    /// </summary>
    public string ManyOfRelatedPaths { get; set; }

    public CaptureValueSynonymSet(object canonicalValue, ExtractedCapture synonymCapture) : this(canonicalValue, synonymCapture.Value)
    {
    }

    public CaptureValueSynonymSet(object canonicalValue, string stringValue = null, string manyOfRelatedPaths = null, int? count = null)
    {
        stringValue ??= canonicalValue.ToString();
        CanonicalValue = canonicalValue;
        CanonicalValueDisplay = ToFriendlyStringOrPattern(canonicalValue);
        ManyOfRelatedPaths = manyOfRelatedPaths;
        SynonymCounts = [];
        IncrementOrSetValueCount(stringValue, count);
    }

    public void IncrementSynonymCapture(ExtractedCapture variantCapture) => IncrementOrSetValueCount(variantCapture.Value);

    public void IncrementOrSetValueCount(string stringValue, int? count = null)
    {
        // Ensure value exists
        SynonymCounts.TryAdd(stringValue, 0);

        if (count.HasValue)
            SynonymCounts[stringValue] = count.Value;
        else
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
*/