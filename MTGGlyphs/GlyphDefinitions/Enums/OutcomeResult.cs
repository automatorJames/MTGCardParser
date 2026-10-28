namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How a choice, requirement or coin flip came out, after "if you" or "if the player": "do" or "does", "don't" or "doesn't", "can't", "win the flip", "lose the flip", "win" or "won", "lose" or "lost" ("if you do, ...", "if you win the flip, ...").</summary>
public enum OutcomeResult
{
    [RegexPattern("does", "do")]
    Did,

    [RegexPattern("doesn't", "don't")]
    DidNot,

    [RegexPattern("can't")]
    Cannot,

    [RegexPattern("win the flip")]
    WonFlip,

    [RegexPattern("lose the flip")]
    LostFlip,

    [RegexPattern("wins?", "won")]
    Won,

    [RegexPattern("loses?", "lost")]
    Lost
}
