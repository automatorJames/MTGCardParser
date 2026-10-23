namespace MTGGlyphs.GlyphDefinitions;

public enum CostPayer
{
    [RegexPattern("you pay")]
    You,

    [RegexPattern("your opponents pay")]
    Opponents
}
