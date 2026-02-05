namespace MTGPlexer;

[Flags]
public enum Proptions
{
    None,
    Plural,
    Optional,
    NoPrecedingSpace,
}

public enum CaptureGroupJoinStrategy
{
    ConcatenateWithSpace,
    AlternateValues,
    CompoundValue
}