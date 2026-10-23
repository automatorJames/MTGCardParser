namespace MTGGlyphs.GlyphDefinitions;

public enum CopyArticle
{
    [RegexPattern("except it's an?", "except it's")]
    ItIs,

    [RegexPattern("and it's an?", "and it's")]
    AndItIs
}
