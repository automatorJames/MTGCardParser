namespace MTGPlexer.TokenAnalysisDTOs.SpanAnalysis;

/// <summary>
/// A branch in the analysis tree, which can contain other nodes.
/// </summary>
public record SpanBranch : SpanAnalysisBase
{
    public Palette Palette { get; init; }
    public bool IsCollapsed { get; init; }

    /// <summary>
    /// Provides an enriched, single-line summary of the node.
    /// </summary>
    public override string ToString()
    {
        string nestedCapture = GetNestedCaptureString();
        string friendlyElementType = ElementType.ToString().ToFriendlyCase();

        // For non-root nodes, showing the character indices is crucial for debugging.
        string captureDisplay = $"[{Start}] \"{nestedCapture}\" [{End}]";

        // Include the critical IsCollapsed status.
        return $"{Path} | {captureDisplay} | {friendlyElementType} | Collapsed: {IsCollapsed} | Children: {Children.Count}";
    }
}