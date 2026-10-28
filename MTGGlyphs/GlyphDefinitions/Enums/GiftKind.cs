namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What the opponent receives from the gift keyword: "a card", "a tapped fish", "a food", "a treasure", "an extra turn", "an octopus" - "gift a card".</summary>
public enum GiftKind
{
    [RegexPattern("a card")]
    Card,

    [RegexPattern("a tapped fish")]
    TappedFish,

    [RegexPattern("a food")]
    Food,

    [RegexPattern("a treasure")]
    Treasure,

    [RegexPattern("an extra turn")]
    ExtraTurn,

    [RegexPattern("an octopus")]
    Octopus
}
