namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How many of the cards this permanent exiled are meant: "a", "all", "all other", "each", "each other", "the" - "a card exiled with this creature", "all cards exiled with this artifact", "each creature card exiled with this land".</summary>
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
