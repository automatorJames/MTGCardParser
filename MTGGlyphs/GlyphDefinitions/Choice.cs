namespace MTGGlyphs.GlyphDefinitions;

public enum Choice
{
    [RegexPattern("an opponent")]
    Opponent,

    [RegexPattern("a player")]
    Player,

    [RegexPattern("a color")]
    Color,

    [RegexPattern("a basic land type")]
    BasicLandType,

    [RegexPattern("a creature type")]
    CreatureType,

    [RegexPattern("a card type")]
    CardType
}
