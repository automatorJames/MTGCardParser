using System.ComponentModel;

namespace Glyphotype;

[Flags]
public enum Proptions
{
    None = 0,
    Plural = 1,
    Optional = 2,
    NoPrecedingSpace = 4,
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

    [Description(",[ ]")]
    CommaSpace,
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

public enum Quantifier
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