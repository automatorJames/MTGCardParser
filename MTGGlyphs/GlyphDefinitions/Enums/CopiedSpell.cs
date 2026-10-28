namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The spell or ability "copy" refers back to: "copy this spell", "copy that spell", "copy that ability", "copy it and you may choose a new target for the copy".</summary>
public enum CopiedSpell
{
    [RegexPattern("this spell")]
    ThisSpell,

    [RegexPattern("that spell")]
    ThatSpell,

    [RegexPattern("that ability")]
    ThatAbility,

    [RegexPattern("it")]
    It
}
