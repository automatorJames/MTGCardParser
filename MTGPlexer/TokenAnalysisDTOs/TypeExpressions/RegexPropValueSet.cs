namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

/// <summary>
/// Represents a distinct property path and all its captured string values,
/// ordered by the frequency of their occurrence.
/// </summary>
public record RegexPropValueSet
(
    TerminalRegexPropPath TerminalRegexPropPath,
    int CaptureGroupPositionStart,
    int CaptureGroupPositionEnd,
    List<CaptureValueVariantSet> ValueCaptureCounts
);