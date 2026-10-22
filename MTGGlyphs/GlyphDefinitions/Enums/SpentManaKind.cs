namespace MTGGlyphs.GlyphDefinitions;

public enum SpentManaKind
{
    Mana,

    [RegexPattern(@"\{s\}")]
    Snow
}
