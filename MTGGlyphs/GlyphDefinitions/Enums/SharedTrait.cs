namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The trait one object shares with another in a condition, after "shares a": "creature type" ("it shares a creature type with this creature", kinship), "color", or "card type".</summary>
public enum SharedTrait
{
    [RegexPattern("creature type")]
    CreatureType,

    Color,

    [RegexPattern("card type")]
    CardType
}
