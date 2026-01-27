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
        var friendlyRaw = rawName.ToFriendlyCase(TitleDisplayOption.Sentence);

        // Check if the current rawName is already the last item in the chain
        bool isRedundant = CurrentNameChain.Count > 0 &&
                           CurrentNameChain.Last().Equals(friendlyRaw, StringComparison.OrdinalIgnoreCase);

        var parts = new List<string>();
        if (!isRedundant)
            parts.Add(friendlyRaw);

        parts.AddRange(CurrentSuffixChain.Select(e => e.ToFriendlyCase(TitleDisplayOption.Sentence)));
        parts.AddRange(localExtensions.Select(e => e.ToFriendlyCase(TitleDisplayOption.Sentence)));

        var formattedBase = string.Join(": ", parts);

        if (CurrentNameChain.Count > 0)
        {
            return string.IsNullOrEmpty(formattedBase)
                ? string.Join(": ", CurrentNameChain)
                : $"{string.Join(": ", CurrentNameChain)}: {formattedBase}";
        }

        return formattedBase;
    }

    public SpanContext PushSuffix(string name) =>
        this with { SuffixChain = new List<string>(CurrentSuffixChain) { name } };

    public SpanContext PushName(string name)
    {
        var friendly = name.ToFriendlyCase(TitleDisplayOption.Sentence);

        // FIX: Prevent pushing the same name twice in a row (e.g., from OneOf variants)
        if (CurrentNameChain.Count > 0 && CurrentNameChain.Last().Equals(friendly, StringComparison.OrdinalIgnoreCase))
            return this;

        return this with { NameChain = new List<string>(CurrentNameChain) { friendly } };
    }

    public SpanContext ClearNameChain() => this with { NameChain = null, SuffixChain = null };
}