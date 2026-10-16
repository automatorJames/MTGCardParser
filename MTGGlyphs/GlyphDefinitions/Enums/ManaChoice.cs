namespace MTGGlyphs.GlyphDefinitions;

public enum ManaChoice
{
    [RegexPattern("any color")]
    AnyColor,

    [RegexPattern("any one color")]
    AnyOneColor,

    [RegexPattern("the chosen color")]
    TheChosenColor
}
