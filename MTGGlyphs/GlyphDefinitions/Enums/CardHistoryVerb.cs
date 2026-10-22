namespace MTGGlyphs.GlyphDefinitions;

public enum CardHistoryVerb
{
    [RegexPattern("cast")]
    Cast,

    [RegexPattern("drawn")]
    Drawn,

    [RegexPattern("cycled or discarded")]
    CycledOrDiscarded,

    [RegexPattern("discarded")]
    Discarded
}
