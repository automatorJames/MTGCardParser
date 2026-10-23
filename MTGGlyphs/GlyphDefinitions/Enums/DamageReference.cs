namespace MTGGlyphs.GlyphDefinitions;

public enum DamageReference
{
    [RegexPattern("that damage")]
    ThatDamage,

    [RegexPattern("the damage dealt")]
    DamageDealt,

    [RegexPattern("that amount of excess damage")]
    ExcessDamage
}
