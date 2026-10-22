namespace MTGGlyphs.GlyphDefinitions;

public enum PlayerSuperlative
{
    [RegexPattern("with the most life")]
    MostLife,

    [RegexPattern("with the highest life total")]
    HighestLifeTotal,

    [RegexPattern("with the lowest life total")]
    LowestLifeTotal,

    [RegexPattern("who has the most cards in hand")]
    MostCardsInHand
}
