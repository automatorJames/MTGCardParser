namespace MTGGlyphs.GlyphDefinitions;

public enum Choice
{
    [RegexPattern("an opponent")]
    Opponent,

    [RegexPattern("a player")]
    Player,

    [RegexPattern("one of your opponents")]
    OneOfYourOpponents,

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

    [RegexPattern("a permanent type")]
    PermanentType,

    [RegexPattern("(?:a|any) card name")]
    CardName,

    [RegexPattern("a nonland card name")]
    NonlandCardName,

    [RegexPattern("a noncreature, nonland card name")]
    NoncreatureNonlandCardName,

    [RegexPattern("a nonartifact, nonland card name")]
    NonartifactNonlandCardName,

    [RegexPattern("a card name other than a basic land card name")]
    NonbasicLandCardName,

    [RegexPattern("a number between [0-9]+ and [0-9]+")]
    NumberInRange,

    [RegexPattern("a number greater than 0")]
    PositiveNumber,

    [RegexPattern("a number")]
    Number,

    [RegexPattern("odd or even")]
    OddOrEven,

    [RegexPattern("any target")]
    AnyTarget
}
