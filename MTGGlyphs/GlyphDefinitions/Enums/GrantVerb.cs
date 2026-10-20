namespace MTGGlyphs.GlyphDefinitions;

public enum GrantVerb
{
    [RegexPattern("has", "have")]
    Has,

    [RegexPattern("gains?")]
    Gains
}
