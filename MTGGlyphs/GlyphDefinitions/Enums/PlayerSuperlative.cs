namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What makes a player the extreme, after "the player": "with the most life", "with the highest life total", "with the lowest life total", "who has the most cards in hand".</summary>
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
