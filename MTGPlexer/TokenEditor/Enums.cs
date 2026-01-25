using System.ComponentModel;

namespace MTGPlexer.TokenEditor;

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

public enum XOfType
{
    None,
    ManyOf,
    OneOf,
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
