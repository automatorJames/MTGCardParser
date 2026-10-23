namespace MTGGlyphs.GlyphDefinitions;

public enum FromAmongCards
{
    [RegexPattern("from among (?:them|those cards|the copies|the (?:revealed|exiled) cards|(?:the )?(?:other |nonland )?cards (?:exiled|revealed) this way)")]
    AmongThem,

    [RegexPattern(@"from among (?:other )?cards exiled with (?:this \w+|it)")]
    AmongExiledWithThis
}
