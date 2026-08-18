using System.ComponentModel;

namespace Glyphotype.GlyphEditor;

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

public enum ShortcutNibMethod
{
    Alt,
    Opt,
    NoSpace,
    Plural
}
