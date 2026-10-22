namespace MTGGlyphs.GlyphDefinitions;

public enum Arrival
{
    [RegexPattern("that (?:was|were) put there")]
    PutThere,

    [RegexPattern("that (?:wasn't|weren't) put there")]
    NotPutThere
}
