namespace MTGPlexer.TokenAnalysis.RegexDTOs;

/// <summary>
/// Represents a distinct property path and all its captured string values,
/// ordered by the frequency of their occurrence.
/// </summary>
public record RegexPropValueSet
(
    string PropPathNameFormatted,
    int CaptureGroupPositionStart,
    int CaptureGroupPositionEnd,
    List<ValueCaptureVariantSet> ValueCaptureCounts
);