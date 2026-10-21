namespace MTGGlyphs.GlyphDefinitions;

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
