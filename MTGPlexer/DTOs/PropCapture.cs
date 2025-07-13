namespace MTGPlexer.DTOs;

public record PropCapture
(
    RegexPropInfo RegexPropInfo,
    TextSpan TextSpan,
    object Value
);

