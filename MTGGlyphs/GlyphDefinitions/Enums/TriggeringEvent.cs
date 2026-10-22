namespace MTGGlyphs.GlyphDefinitions;

public enum TriggeringEvent
{
    [RegexPattern("entering the battlefield", "entering")]
    Entering,

    [RegexPattern("dying")]
    Dying,

    [RegexPattern("attacking")]
    Attacking
}
