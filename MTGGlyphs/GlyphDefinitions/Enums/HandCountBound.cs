namespace MTGGlyphs.GlyphDefinitions;

public enum HandCountBound
{
    Exactly,

    [RegexPattern("fewer than", "less than")]
    FewerThan,

    MoreThan
}
