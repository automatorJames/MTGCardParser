namespace MTGGlyphs.GlyphDefinitions;

public enum StateTense
{
    [RegexPattern("is")]
    Present,

    [RegexPattern("was")]
    Past
}
