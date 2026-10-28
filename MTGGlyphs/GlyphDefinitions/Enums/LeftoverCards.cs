namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The cards left over from a look-at or reveal effect, as the object of "put": "the rest", "the rest of the cards", "the rest of the revealed cards", "all cards revealed this way".</summary>
public enum LeftoverCards
{
    [RegexPattern("the rest")]
    TheRest,

    [RegexPattern("the rest of (?:the|those) cards")]
    TheRestOfTheCards,

    [RegexPattern("the rest of the (?:revealed|exiled) cards", "the rest of the cards (?:revealed|exiled) this way")]
    TheRestRevealed,

    [RegexPattern("all cards (?:revealed|exiled) this way", "the cards you revealed this way")]
    AllRevealed
}
