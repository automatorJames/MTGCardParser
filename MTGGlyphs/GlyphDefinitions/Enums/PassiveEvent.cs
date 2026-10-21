namespace MTGGlyphs.GlyphDefinitions;

public enum PassiveEvent
{
    Destroyed,

    [RegexPattern("dealt damage")]
    DealtDamage,

    Countered
}
