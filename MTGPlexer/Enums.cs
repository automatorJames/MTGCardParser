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

public enum ContextActionType
{
    Delete,
    ConvertToOneOf,
    ConvertToManyOf,
    ConvertToCompoundOf
}

public enum XOfType
{
    None,
    ManyOf,
    CompoundOf,
    OptionalOf,
    DynamicOf,
}

public enum ShortcutSnippetMethod
{
    Alt,
    Opt,
    NoSpace,
    Plural
}
