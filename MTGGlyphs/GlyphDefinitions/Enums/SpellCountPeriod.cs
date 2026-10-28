namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The period over which a player's spells are counted for an nth-spell effect: "each turn" ("your second spell each turn"), "during each opponent's turn", or "in a turn" ("your third spell in a turn").</summary>
public enum SpellCountPeriod
{
    [RegexPattern("each turn")]
    EachTurn,

    [RegexPattern("during each opponent's turn")]
    EachOpponentsTurn,

    [RegexPattern("in a turn")]
    InATurn
}
