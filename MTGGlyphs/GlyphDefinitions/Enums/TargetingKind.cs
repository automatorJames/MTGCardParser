namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What's barred from targeting something: "spells or abilities", "spells or activated abilities", "spells" ("blue or black spells"), or "abilities" ("abilities from artifact sources").</summary>
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
