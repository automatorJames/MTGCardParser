namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A fraction or multiple applied to an amount: "half" ("half your life, rounded up"), "twice" ("twice as much of that mana"), "a third of", or "three times" ("three times as much of that mana").</summary>
public enum ScaleFactor
{
    Half,

    Twice,

    [RegexPattern("a third of")]
    AThird,

    [RegexPattern("three times")]
    ThreeTimes
}
