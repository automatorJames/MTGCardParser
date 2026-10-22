namespace MTGGlyphs.GlyphDefinitions;

public enum AddedManaAmount
{
    [RegexPattern("an additional")]
    Additional,

    [RegexPattern("that much")]
    ThatMuch
}
