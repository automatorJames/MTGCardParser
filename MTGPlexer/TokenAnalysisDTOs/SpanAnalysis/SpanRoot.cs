namespace MTGPlexer.TokenAnalysisDTOs.SpanAnalysis;

/// <summary>
/// The root of the analysis tree.
/// </summary>
public record SpanRoot : SpanBranch
{
    public string OriginalFullText { get; init; }
    public TokenUnit RootToken { get; init; }
    public string CardName { get; init; }
    public int ClauseIndex { get; init; }

    /// <summary>
    /// Provides an enriched, single-line summary of the node.
    /// </summary>
    public override string ToString()
    {
        string nestedCapture = GetNestedCaptureString();
        string friendlyElementType = ElementType.ToString().ToFriendlyCase();
        string captureDisplay = $"\"{nestedCapture}\"";
        return $"{CapturePath} | {captureDisplay} | {friendlyElementType} | Children: {Children.Count}";
    }
}