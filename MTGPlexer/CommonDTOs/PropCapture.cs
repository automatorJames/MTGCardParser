namespace MTGPlexer.CommonDTOs;

public record PropCapture
(
    RegexPropInfo RegexPropInfo,
    TextSpan TextSpan,
    object Value
);

