namespace MTGGlyphs.GlyphDefinitions;

public enum SpellRelation
{
    [RegexPattern("casts?")]
    Cast,

    [RegexPattern("controls?")]
    Control
}
