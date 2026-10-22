namespace MTGGlyphs.GlyphDefinitions;

public enum LifeChange
{
    [RegexPattern("gained", "have gained")]
    Gained,

    [RegexPattern("lost", "have lost")]
    Lost
}
