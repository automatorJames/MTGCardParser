namespace MTGGlyphs.GlyphDefinitions;

public enum CastFrom
{
    [RegexPattern("from your hand")]
    Hand,

    [RegexPattern("from among (?:them|those cards)")]
    AmongThem,

    [RegexPattern("from exile")]
    Exile,

    [RegexPattern("from your graveyard")]
    Graveyard
}
