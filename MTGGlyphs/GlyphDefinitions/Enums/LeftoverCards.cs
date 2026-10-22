namespace MTGGlyphs.GlyphDefinitions;

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
