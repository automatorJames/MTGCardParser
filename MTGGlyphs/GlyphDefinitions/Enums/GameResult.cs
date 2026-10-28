namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"wins" or "loses" the game: "you lose the game", "that player wins the game", "your opponents can't win the game".</summary>
public enum GameResult
{
    [RegexPattern("loses?")]
    Lose,

    [RegexPattern("wins?")]
    Win
}
