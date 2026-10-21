namespace MTGGlyphs.GlyphDefinitions;

public enum Choice
{
    [RegexPattern("an opponent")]
    Opponent,

    [RegexPattern("a player")]
    Player,

    [RegexPattern("a color")]
    Color,

    [RegexPattern("two colors")]
    TwoColors,

    [RegexPattern("a color word")]
    ColorWord,

    [RegexPattern("a basic land type")]
    BasicLandType,

    [RegexPattern("two basic land types")]
    TwoBasicLandTypes,

    [RegexPattern("a land type")]
    LandType,

    [RegexPattern("a creature type")]
    CreatureType,

    [RegexPattern("a card type")]
    CardType,

    [RegexPattern("(?:a|any) card name")]
    CardName,

    [RegexPattern("a nonland card name")]
    NonlandCardName,

    [RegexPattern("a noncreature, nonland card name")]
    NoncreatureNonlandCardName,

    [RegexPattern("a card name other than a basic land card name")]
    NonbasicLandCardName,

    [RegexPattern("a number")]
    Number,

    [RegexPattern("odd or even")]
    OddOrEven,

    [RegexPattern("any target")]
    AnyTarget
}
