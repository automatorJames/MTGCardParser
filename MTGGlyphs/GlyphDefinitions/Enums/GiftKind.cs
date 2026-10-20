namespace MTGGlyphs.GlyphDefinitions;

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
