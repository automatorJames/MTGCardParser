namespace MTGGlyphs.GlyphDefinitions;

public enum GameResult
{
    [RegexPattern("loses?")]
    Lose,

    [RegexPattern("wins?")]
    Win
}
