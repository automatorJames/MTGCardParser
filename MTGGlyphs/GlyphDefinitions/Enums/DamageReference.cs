namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Damage dealt earlier in the text, as an amount a characteristic is bounded by: "that damage", "the damage dealt", "that amount of excess damage" - "with mana value less than or equal to that damage".</summary>
public enum DamageReference
{
    [RegexPattern("that damage")]
    ThatDamage,

    [RegexPattern("the damage dealt")]
    DamageDealt,

    [RegexPattern("that amount of excess damage")]
    ExcessDamage
}
