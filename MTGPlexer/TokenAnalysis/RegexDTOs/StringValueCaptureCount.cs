namespace MTGPlexer.TokenAnalysis.RegexDTOs;

/// <summary>
/// Represents a single string value and the number of times it was captured.
/// </summary>
public record StringValueCaptureCount
(
    string StringValue,
    int CaptureCount
);