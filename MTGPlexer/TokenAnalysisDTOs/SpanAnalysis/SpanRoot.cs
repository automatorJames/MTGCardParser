namespace MTGPlexer.TokenAnalysisDTOs.SpanAnalysis;

/// <summary>
/// The specialized root of the analysis tree containing global context.
/// </summary>
public record SpanRoot : SpanBranch
{
    public string OriginalFullText { get; init; } = string.Empty;
    public TokenUnit RootToken { get; init; } = null!;
    public string CardName { get; init; } = string.Empty;
    public int ClauseIndex { get; init; }

    public override string ToString()
    {
        return $"{CapturePath} | \"{GetNestedCaptureString()}\" | {ElementType} | Card: {CardName} | Children: {Children.Count}";
    }
}