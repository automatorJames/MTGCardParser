namespace MTGPlexer;

public enum CaptureTypeVariant
{
    TokenUnit,
    Enum
}

public enum MatchStatus 
{
    None, 
    Partial, 
    Full 
}

public enum SpanClass
{
    keyword,
    type,
    enumtype,
    identifier,
    method,
    stringliteral
}