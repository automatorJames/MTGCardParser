namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What objects have in common, with its article: "a creature type", "a color", "a card type", or "a permanent type", as in "that share a card type" or "that share a color with it".</summary>
public enum SharedAspect
{
    [RegexPattern("a creature type")]
    CreatureType,

    [RegexPattern("a color")]
    Color,

    [RegexPattern("a card type")]
    CardType,

    [RegexPattern("a permanent type")]
    PermanentType
}
