namespace MTGGlyphs.GlyphDefinitions;

public enum FromAmongCards
{
    [RegexPattern("from among (?:them|those cards|the (?:revealed|exiled) cards|cards (?:exiled|revealed) this way)")]
    AmongThem,

    [RegexPattern(@"from among (?:other )?cards exiled with (?:this \w+|it)")]
    AmongExiledWithThis
}
