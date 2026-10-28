namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The set of cards spells are cast from: "from among them", "from among those cards", "from among the exiled cards", "from among cards exiled this way", or "from among cards exiled with this creature" - "cast an instant or sorcery spell from among them".</summary>
public enum FromAmongCards
{
    [RegexPattern("from among (?:them|those cards|the copies|the (?:revealed|exiled) cards|(?:the )?(?:other |nonland )?cards (?:exiled|revealed) this way)")]
    AmongThem,

    [RegexPattern(@"from among (?:other )?cards exiled with (?:this \w+|it)")]
    AmongExiledWithThis
}
