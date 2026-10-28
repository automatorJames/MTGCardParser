namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Who places counters in a counter replacement effect, when it isn't a player: "an effect", "it", "they" - "if an effect would put one or more counters on a permanent you control, it puts twice that many of those counters on that permanent instead".</summary>
public enum CounterPlacer
{
    [RegexPattern("an effect")]
    Effect,

    It,

    They
}
