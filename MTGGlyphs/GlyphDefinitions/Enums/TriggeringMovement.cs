namespace MTGGlyphs.GlyphDefinitions;

public enum TriggeringMovement
{
    [RegexPattern("entering(?: the battlefield)?")]
    Entering,

    [RegexPattern("dying")]
    Dying
}
