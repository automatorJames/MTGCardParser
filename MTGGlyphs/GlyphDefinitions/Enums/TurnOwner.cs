namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whose turns a static effect is limited to, after "during": "your turn", "each opponent's turn" or "an opponent's turn", or "turns other than yours".</summary>
public enum TurnOwner
{
    [RegexPattern("your turn")]
    YourTurn,

    [RegexPattern("each opponent's turn", "an opponent's turn")]
    OpponentsTurn,

    [RegexPattern("turns other than yours")]
    OtherTurns
}
