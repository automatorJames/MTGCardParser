using System.ComponentModel;

namespace Glyphotype;

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

/// <summary>Whether a referent is one thing or several - what an <see cref="GlyphPrimitives.BackReference"/> such as "it" or "they" has to agree with.</summary>
public enum GrammaticalNumber
{
    /// <summary>Not known, or not declared - agrees with either.</summary>
    Unspecified,

    Singular,
    Plural,
}
