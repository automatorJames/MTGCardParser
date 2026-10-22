namespace MTGGlyphs.GlyphDefinitions;

public enum RetargetedObject
{
    [RegexPattern("the copy")]
    TheCopy,

    [RegexPattern("that copy")]
    ThatCopy,

    [RegexPattern("their copy")]
    TheirCopy,

    [RegexPattern("the copies")]
    TheCopies,

    [RegexPattern("that spell")]
    ThatSpell,

    [RegexPattern("the spell")]
    TheSpell,

    [RegexPattern("it")]
    It
}
