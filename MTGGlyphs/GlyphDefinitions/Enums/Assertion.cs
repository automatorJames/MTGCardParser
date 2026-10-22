namespace MTGGlyphs.GlyphDefinitions;

public enum Assertion
{
    Is,

    [RegexPattern("isn't")]
    Isnt,

    Was,

    [RegexPattern("wasn't")]
    Wasnt
}
