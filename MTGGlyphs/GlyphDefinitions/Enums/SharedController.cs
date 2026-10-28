namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A targeting restriction tying several targets' controllers together, after "controlled by": "the same player", "the same opponent", or "different players".</summary>
public enum SharedController
{
    [RegexPattern("the same player")]
    SamePlayer,

    [RegexPattern("the same opponent")]
    SameOpponent,

    [RegexPattern("different players")]
    DifferentPlayers
}
