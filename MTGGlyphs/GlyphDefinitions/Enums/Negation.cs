namespace MTGGlyphs.GlyphDefinitions;

public enum Negation
{
    [RegexPattern("can't")]
    Cant,

    [RegexPattern("don't", "doesn't")]
    Dont
}
