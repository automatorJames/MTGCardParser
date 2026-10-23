namespace MTGGlyphs.GlyphDefinitions;

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
