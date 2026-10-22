namespace MTGGlyphs.GlyphDefinitions;

public enum TargetingKind
{
    [RegexPattern("spells or abilities")]
    SpellsOrAbilities,

    [RegexPattern("spells or activated abilities")]
    SpellsOrActivatedAbilities,

    [RegexPattern("spells")]
    Spells,

    [RegexPattern("abilities")]
    Abilities
}
