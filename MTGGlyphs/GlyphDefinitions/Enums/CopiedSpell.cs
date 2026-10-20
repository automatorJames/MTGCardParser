namespace MTGGlyphs.GlyphDefinitions;

public enum CopiedSpell
{
    [RegexPattern("this spell")]
    ThisSpell,

    [RegexPattern("that spell")]
    ThatSpell,

    [RegexPattern("it")]
    It
}
