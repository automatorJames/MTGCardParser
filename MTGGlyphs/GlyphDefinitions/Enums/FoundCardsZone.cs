namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Where cards found by a search are put: "into your hand" (or "their"), "into your graveyard", in "put it into your hand", "put that card into your graveyard".</summary>
public enum FoundCardsZone
{
    [RegexPattern("into (?:your|their) hand")]
    Hand,

    [RegexPattern("into (?:your|their) graveyard")]
    Graveyard
}
