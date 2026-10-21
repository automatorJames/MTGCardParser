namespace MTGGlyphs.GlyphDefinitions;

public enum FoundCardsZone
{
    [RegexPattern("into (?:your|their) hand")]
    Hand,

    [RegexPattern("into (?:your|their) graveyard")]
    Graveyard
}
