namespace MTGGlyphs.GlyphDefinitions;

public enum ManaChoice
{
    [RegexPattern("any color")]
    AnyColor,

    [RegexPattern("any one color")]
    AnyOneColor,

    [RegexPattern("different colors")]
    DifferentColors,

    [RegexPattern("the chosen color")]
    TheChosenColor,

    [RegexPattern("that color")]
    ThatColor,

    [RegexPattern("any color in your commander's color identity")]
    CommanderColorIdentity,

    [RegexPattern("any of the exiled card's colors")]
    ExiledCardsColors
}
