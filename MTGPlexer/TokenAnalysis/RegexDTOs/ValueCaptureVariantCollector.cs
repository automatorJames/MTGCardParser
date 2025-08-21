namespace MTGPlexer.TokenAnalysis.RegexDTOs;

public class ValueCaptureVariantCollector
{
    public string CanonicalRepresentation { get; set; }
    public Dictionary<string, int> VariantCounts { get; set; }

    public ValueCaptureVariantCollector(string canonicalRepresentation, string variant)
    {
        CanonicalRepresentation = canonicalRepresentation;
        VariantCounts = new Dictionary<string, int> { [variant] = 1 };
    }

    public void IncrementVariant(string variant)
    {
        if (!VariantCounts.ContainsKey(variant))
            VariantCounts[variant] = 1;
        else
            VariantCounts[variant]++;
    }
}

