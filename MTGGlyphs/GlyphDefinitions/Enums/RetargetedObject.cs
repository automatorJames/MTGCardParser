namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The spell whose targets may be changed, after "choose new targets for": "the copy", "that copy", "their copy", "the copies" (storm's reminder), "that spell", "the spell", or "it".</summary>
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
