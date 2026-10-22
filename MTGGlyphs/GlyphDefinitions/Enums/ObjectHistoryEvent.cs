namespace MTGGlyphs.GlyphDefinitions;

public enum ObjectHistoryEvent
{
    [RegexPattern("was kicked")]
    WasKicked,

    [RegexPattern("has mutated")]
    HasMutated
}
