namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Which color produced mana is when a player or an earlier choice picks it, after "mana of": "any color", "any one color", "the chosen color", "that color", "different colors", "any color in your commander's color identity", "any of the exiled card's colors".</summary>
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
