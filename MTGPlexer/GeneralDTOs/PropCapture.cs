namespace MTGPlexer.GeneralDTOs;

public record PropCapture
(
    RegexPropInfo RegexPropInfo,
    TextSpan TextSpan,
    object Value
);

