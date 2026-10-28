namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A multiplier on an amount a replacement effect just named: "double that damage", "twice that much life", "triple that damage", "three times".</summary>
public enum AmountFactor
{
    [RegexPattern("twice", "double")]
    Twice,

    [RegexPattern("triple", "three times")]
    Thrice
}
