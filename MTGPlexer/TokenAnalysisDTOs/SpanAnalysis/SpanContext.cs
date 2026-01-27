internal record SpanContext(
    string FullText,
    string PathPrefix,
    IReadOnlyList<string> NameChain = null,
    IReadOnlyList<string> SuffixChain = null)
{
    public IReadOnlyList<string> CurrentNameChain => NameChain ?? Array.Empty<string>();
    public IReadOnlyList<string> CurrentSuffixChain => SuffixChain ?? Array.Empty<string>();

    public string FormatName(string rawName, params string[] localExtensions)
    {
        var parts = new List<string> { rawName.ToFriendlyCase(TitleDisplayOption.Sentence) };

        parts.AddRange(CurrentSuffixChain.Select(e => e.ToFriendlyCase(TitleDisplayOption.Sentence)));
        parts.AddRange(localExtensions.Select(e => e.ToFriendlyCase(TitleDisplayOption.Sentence)));

        var nameWithSuffixes = string.Join(": ", parts);

        return CurrentNameChain.Count > 0
            ? $"{string.Join(": ", CurrentNameChain)}: {nameWithSuffixes}"
            : nameWithSuffixes;
    }

    public SpanContext PushSuffix(string name) =>
        this with { SuffixChain = new List<string>(CurrentSuffixChain) { name } };

    public SpanContext PushName(string name) =>
        this with { NameChain = new List<string>(CurrentNameChain) { name.ToFriendlyCase(TitleDisplayOption.Sentence) } };

    // Clear everything: used when a visible branch is reached
    public SpanContext Clear() => this with { NameChain = null, SuffixChain = null };
}