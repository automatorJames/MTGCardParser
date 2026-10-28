namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How a replacement effect scales counters being put or tokens being created: "twice that many", "three times that many", "that many plus one", "half that many" (Doubling Season, Hardened Scales).</summary>
public enum CounterIncrease
{
    [RegexPattern("twice that many")]
    Double,

    [RegexPattern("three times that many")]
    Triple,

    [RegexPattern("that many plus one")]
    PlusOne,

    [RegexPattern("half that many")]
    Half
}
