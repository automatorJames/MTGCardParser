namespace MTGGlyphs.GlyphDefinitions;

public enum SpellCountPeriod
{
    [RegexPattern("each turn")]
    EachTurn,

    [RegexPattern("during each opponent's turn")]
    EachOpponentsTurn,

    [RegexPattern("in a turn")]
    InATurn
}
