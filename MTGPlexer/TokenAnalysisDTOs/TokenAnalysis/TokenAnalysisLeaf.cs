namespace MTGPlexer.TokenAnalysisDTOs.TokenAnalysis;

/// <summary>
/// A leaf in the analysis tree, representing a primary terminal value.
/// </summary>
public record TokenAnalysisLeaf : TokenAnalysisBase
{
    public Palette Palette { get; init; }
    public string TerminalValString { get; init; }
    public string TerminalType { get; init; }

    /// <summary>
    /// Provides an enriched, single-line summary of the node.
    /// </summary>
    public override string ToString()
    {
        // For a leaf, GetNestedCaptureString() will simply be its own CaptureTextOriginal.
        string nestedCapture = GetNestedCaptureString();
        string friendlyElementType = ElementType.ToString().ToFriendlyCase();
        string captureDisplay = $"[{Start}] \"{nestedCapture}\" [{End}]";

        // Display the captured terminal value and its type.
        return $"{Path} | {captureDisplay} | {friendlyElementType} | Value: \"{TerminalValString}\" ({TerminalType})";
    }
}