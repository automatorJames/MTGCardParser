namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The kind of ability, or spell or ability, a targeted stack object can be, after "target": "activated ability", "triggered ability", "activated or triggered ability", "spell or ability", "spell, activated ability, or triggered ability", or just "ability".</summary>
public enum StackAbilityKind
{
    [RegexPattern("spell, activated ability, or triggered ability")]
    SpellActivatedOrTriggeredAbility,

    [RegexPattern("spell or ability")]
    SpellOrAbility,

    [RegexPattern("activated or triggered ability")]
    ActivatedOrTriggeredAbility,

    ActivatedAbility,

    TriggeredAbility,

    Ability
}
