namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An action done to half of something, with rounding: "mills" ("mills half their library, rounded down"), "discards" ("discards half the cards in their hand, rounded up"), "sacrifices".</summary>
public enum HalvingAction
{
    [RegexPattern("mills?")]
    Mill,

    [RegexPattern("discards?")]
    Discard,

    [RegexPattern("sacrifices?")]
    Sacrifice
}
