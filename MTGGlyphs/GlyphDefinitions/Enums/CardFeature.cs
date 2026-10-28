namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A feature a spell is narrowed by having that isn't a keyword: "that has an adventure" (beside "that has flash", a keyword).</summary>
public enum CardFeature
{
    [RegexPattern("an adventure")]
    Adventure
}
