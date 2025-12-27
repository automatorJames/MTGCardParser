namespace MTGPlexer.TokenAnalysisDTOs.SpanAnalysis;

/// <summary>
/// Internal state bag used during the recursive build process.
/// Handles coordinate offsets, path prefixes, and the naming chain for collapsed nodes.
/// </summary>
internal record SpanContext(
    string FullText,
    string PathPrefix,
    IReadOnlyList<string> NameChain = null)
{
    public IReadOnlyList<string> CurrentNameChain => NameChain ?? Array.Empty<string>();

    public SpanContext PushName(string name) =>
        this with { NameChain = new List<string>(CurrentNameChain) { name } };

    public SpanContext ClearNameChain() => this with { NameChain = null };
}