namespace MTGPlexer.TokenAnalysisDTOs.TokenAnalysis;

/// <summary>
/// A sub-leaf, representing a secondary or derived terminal value.
/// It is a distinct type from a Leaf and inherits directly from the base.
/// </summary>
public record TokenAnalysisSubLeaf : TokenAnalysisLeaf
{
    /// <summary>
    /// Provides an enriched, single-line summary of the node.
    /// </summary>
    public override string ToString()
    {
        string nestedCapture = GetNestedCaptureString();
        string friendlyElementType = ElementType.ToString().ToFriendlyCase();
        string captureDisplay = $"[{Start}] \"{nestedCapture}\" [{End}]";

        // Display the captured terminal value and its type, same as a regular leaf.
        return $"{Path} | {captureDisplay} | {friendlyElementType} | Value: \"{TerminalValString}\" ({TerminalType})";
    }
}