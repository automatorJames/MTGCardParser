namespace MTGGlyphs.GlyphDefinitions;

public enum ManaChoice
{
    [RegexPattern("any color")]
    AnyColor,

    [RegexPattern("any one color")]
    AnyOneColor,

    [RegexPattern("the chosen color")]
    TheChosenColor,

    [RegexPattern("that color")]
    ThatColor,

    [RegexPattern("any type that land produced")]
    AnyTypeThatLandProduced,

    [RegexPattern("any type that land could produce")]
    AnyTypeThatLandCouldProduce,

    [RegexPattern("any type that a land you control could produce")]
    AnyTypeYourLandsCouldProduce,

    [RegexPattern("any color that a land an opponent controls could produce")]
    AnyColorOpponentsLandsCouldProduce,

    [RegexPattern("any color in your commander's color identity")]
    CommanderColorIdentity,

    [RegexPattern("any of the exiled card's colors")]
    ExiledCardsColors
}
