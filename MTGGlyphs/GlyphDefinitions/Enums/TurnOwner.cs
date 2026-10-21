namespace MTGGlyphs.GlyphDefinitions;

public enum TurnOwner
{
    [RegexPattern("your turn")]
    YourTurn,

    [RegexPattern("each opponent's turn", "an opponent's turn")]
    OpponentsTurn,

    [RegexPattern("turns other than yours")]
    OtherTurns
}
