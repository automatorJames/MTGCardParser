namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Something done to a permanent or spell, said passively after "would be": "destroyed", "dealt damage", "countered" ("if it would be destroyed", "would be dealt damage or destroyed").</summary>
public enum PassiveEvent
{
    Destroyed,

    [RegexPattern("dealt damage")]
    DealtDamage,

    Countered
}
