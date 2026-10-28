namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A spell, or its card, referred back to: "that spell" (or "the spell"), "that spell or ability", "this spell", "a spell cast this way", or "that card", as in "if that spell would be put into a graveyard, exile it instead" and "with that spell's mana value".</summary>
public enum SpellReference
{
    [RegexPattern("that spell or ability")]
    ThatSpellOrAbility,

    [RegexPattern("that spell", "the spell")]
    ThatSpell,

    [RegexPattern("a spell cast this way")]
    SpellCastThisWay,

    [RegexPattern("this spell")]
    ThisSpell,

    [RegexPattern("that card")]
    ThatCard
}
