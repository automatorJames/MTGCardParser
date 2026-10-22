namespace MTGGlyphs.GlyphDefinitions;

public enum SharedAspect
{
    [RegexPattern("a creature type")]
    CreatureType,

    [RegexPattern("a color")]
    Color,

    [RegexPattern("a card type")]
    CardType
}
