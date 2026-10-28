namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The exemption for mana abilities on a cost increase: "unless they're mana abilities" in "activated abilities cost {2} more to activate unless they're mana abilities".</summary>
public enum ManaAbilityExemption
{
    [RegexPattern("unless they're mana abilities")]
    UnlessManaAbilities
}
