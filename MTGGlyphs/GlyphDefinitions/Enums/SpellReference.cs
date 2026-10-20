namespace MTGGlyphs.GlyphDefinitions;

public enum SpellReference
{
    [RegexPattern("that spell")]
    ThatSpell,

    [RegexPattern("a spell cast this way")]
    SpellCastThisWay,

    [RegexPattern("this spell")]
    ThisSpell,

    [RegexPattern("that card")]
    ThatCard
}
