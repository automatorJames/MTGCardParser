namespace MTGGlyphs.GlyphDefinitions;

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
