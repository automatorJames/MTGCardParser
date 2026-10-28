namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The clause ending a token description that says the tokens enter in combat: "that's attacking"/"that are attacking", "that's tapped and attacking"/"that are tapped and attacking" - "two 1/1 white soldier creature tokens that are tapped and attacking".</summary>
public enum EntersAttacking
{
    [RegexPattern("that's tapped and attacking", "that are tapped and attacking")]
    TappedAndAttacking,

    [RegexPattern("that's attacking", "that are attacking")]
    Attacking
}
