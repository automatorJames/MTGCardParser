namespace MTGPlexer.TokenAnalysisDTOs.SpanAnalysis;

internal record SpanContext(string FullText, string PathPrefix, IReadOnlyList<string> NameChain = null)
{
    private IReadOnlyList<string> CurrentNameChain => NameChain ?? Array.Empty<string>();

    /// <summary>
    /// Combines the inherited name chain with the current node's name.
    /// Handles Role vs Identity redundancy (e.g., preventing "Apple: Apple").
    /// </summary>
    public string FormatName(string rawName)
    {
        var friendly = rawName.ToFriendlyCase(TitleDisplayOption.Sentence);

        // If the chain already ends with this name, don't repeat it
        if (CurrentNameChain.Count > 0 && CurrentNameChain.Last().Equals(friendly, StringComparison.OrdinalIgnoreCase))
        {
            return string.Join(": ", CurrentNameChain);
        }

        return CurrentNameChain.Count > 0
            ? $"{string.Join(": ", CurrentNameChain)}: {friendly}"
            : friendly;
    }

    public SpanContext PushName(string name)
    {
        return this with
        {
            NameChain = new List<string>(CurrentNameChain) { name.ToFriendlyCase(TitleDisplayOption.Sentence) }
        };
    }

    public SpanContext Clear()
    {
        return this with { NameChain = null };
    }
}