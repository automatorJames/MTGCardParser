namespace MTGGlyphs.GlyphDefinitions;

public enum EntersAttacking
{
    [RegexPattern("that's tapped and attacking", "that are tapped and attacking")]
    TappedAndAttacking,

    [RegexPattern("that's attacking", "that are attacking")]
    Attacking
}
