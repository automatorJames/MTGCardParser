internal record SpanContext(
    string FullText,
    string PathPrefix,
    IReadOnlyList<string> NameChain = null,
    IReadOnlyList<string> SuffixChain = null) // Added Suffixes
{
    public IReadOnlyList<string> CurrentNameChain => NameChain ?? Array.Empty<string>();
    public IReadOnlyList<string> CurrentSuffixChain => SuffixChain ?? Array.Empty<string>();

    public string FormatName(string rawName, params string[] localExtensions)
    {
        var parts = new List<string> { rawName.ToFriendlyCase(TitleDisplayOption.Sentence) };

        // 1. Add context-level suffixes first, then local ones
        parts.AddRange(CurrentSuffixChain.Select(e => e.ToFriendlyCase(TitleDisplayOption.Sentence)));
        parts.AddRange(localExtensions.Select(e => e.ToFriendlyCase(TitleDisplayOption.Sentence)));

        var nameWithSuffixes = string.Join(": ", parts);

        // 2. Prepend the inherited chain
        return CurrentNameChain.Count > 0
            ? $"{string.Join(": ", CurrentNameChain)}: {nameWithSuffixes}"
            : nameWithSuffixes;
    }

    public SpanContext PushSuffix(string name) =>
        this with { SuffixChain = new List<string>(CurrentSuffixChain) { name } };

    public SpanContext PushName(string name) =>
        this with { NameChain = new List<string>(CurrentNameChain) { name } };

    // Clear both so suffixes are "one-shot" for the immediate child
    public SpanContext ClearNameChain() => this with { NameChain = null, SuffixChain = null };
}