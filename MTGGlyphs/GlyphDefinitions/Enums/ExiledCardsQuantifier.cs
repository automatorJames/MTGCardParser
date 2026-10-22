namespace MTGGlyphs.GlyphDefinitions;

public enum ExiledCardsQuantifier
{
    [RegexPattern("an?")]
    A,

    [RegexPattern("all other")]
    AllOther,

    [RegexPattern("each other")]
    EachOther,

    All,

    Each,

    The
}
