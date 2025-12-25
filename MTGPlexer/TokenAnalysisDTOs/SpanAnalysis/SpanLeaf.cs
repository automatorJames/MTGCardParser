namespace MTGPlexer.TokenAnalysisDTOs.SpanAnalysis;

/// <summary>
/// A terminal node representing a specific value (enum, bool, string).
/// </summary>
public record SpanLeaf : SpanNode
{
    public Palette Palette { get; init; } = null!;
    public string TerminalValString { get; init; } = string.Empty;
    public string TerminalType { get; init; } = string.Empty;

    public override string ToString()
    {
        return $"{CapturePath} | [{Start}] \"{CaptureTextOriginal}\" [{End}] | {ElementType} | Value: \"{TerminalValString}\" ({TerminalType})";
    }
}