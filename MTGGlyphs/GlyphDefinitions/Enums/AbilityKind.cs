namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A keyworded kind of ability, in "each … ability" on an object: "each exhaust ability", "each power-up ability", "each loyalty ability".</summary>
public enum AbilityKind
{
    Exhaust,

    [RegexPattern("power-up")]
    PowerUp,

    Loyalty
}
