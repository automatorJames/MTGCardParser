namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What an object or player becomes the target of, with its article: "a spell or ability" (ward's reminder, heroic triggers), "a spell", "an aura spell", "an instant or sorcery spell", or "an ability"; also what "causes you to discard this card".</summary>
public enum TargetingSource
{
    [RegexPattern("a spell or ability")]
    SpellOrAbility,

    [RegexPattern("a spell")]
    Spell,

    [RegexPattern("an aura spell")]
    AuraSpell,

    [RegexPattern("an instant or sorcery spell")]
    InstantOrSorcerySpell,

    [RegexPattern("an ability")]
    Ability
}
