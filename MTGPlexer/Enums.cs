using System.ComponentModel;

namespace MTGPlexer;

[Flags]
public enum Proptions
{
    None,
    Plural,
    Optional,
    OneOrMore,
    NoPrecedingSpace,
}

public enum CaptureGroupJoinStrategy
{
    ConcatenateWithSpace,
    AlternateValues,
    CompoundValue
}

public enum Joiner
{
    [Description("")]
    None,

    [Description("[ ]")]
    Space,

    [Description("|")]
    Pipe,

    [Description("-")]
    Dash,

    [Description("_")]
    Underscore,

    [Description(".")]
    Dot,
}

public enum MultiItemOrdinal
{
    First,
    SecondPlus,
    Last
}

public enum OneOfItemOrdinal
{
    First,
    Second,
    Third
}

public enum Conjunction
{
    And,
    Or
}

public enum GroupQuantifier
{
    [Description("*")]
    AnyNumber,

    [Description("+")]
    OneOrMore,

    [Description("{2,}")]
    TwoOrMore,

    [Description("?")]
    Optional
}