namespace MTGGlyphs.GlyphDefinitions;

public enum TurnSpan
{
    [RegexPattern("this turn")]
    ThisTurn,

    [RegexPattern("last turn")]
    LastTurn
}
