namespace MTGPlexer;

[Flags]
public enum Proptions
{
    None,
    Plural,
    Optional,
    NoPrecedingSpace,
}

public enum CaptureTypeVariant
{
    TokenUnit,
    Enum
}