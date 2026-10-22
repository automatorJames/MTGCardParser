namespace MTGGlyphs.GlyphDefinitions;

public enum SharedController
{
    [RegexPattern("the same player")]
    SamePlayer,

    [RegexPattern("the same opponent")]
    SameOpponent,

    [RegexPattern("different players")]
    DifferentPlayers
}
