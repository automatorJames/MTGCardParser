namespace MTGPlexer.TokenAnalysis.RegexDTOs;

public record RegexPropValueSet
(
    string PropPathNameFormatted,
    List<StringValueCaptureCount> ValueCaptureCounts
);

