namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The bare noun for abilities of no stated keyword, as "activate" takes it: "activate abilities", "activate an ability of an elemental".</summary>
public enum AbilitiesWord
{
    Abilities,

    [RegexPattern("an ability")]
    AnAbility
}
