namespace MTGPlexer.TokenAnalysis.RegexDTOs;

public record RegexCapturePosition
(
    string Capture,
    int Start,
    int End
);

