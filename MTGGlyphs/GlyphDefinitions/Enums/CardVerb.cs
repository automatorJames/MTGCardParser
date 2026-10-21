namespace MTGGlyphs.GlyphDefinitions;

[OptionalPlural]
public enum CardVerb
{
    Draw,

    [RegexPattern("discard", "discarding")]
    Discard
}
