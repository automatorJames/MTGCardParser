namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A card type, as it qualifies cards, spells, permanents and tokens or is chosen and compared: "artifact", "creature", "instant", "planeswalker", "battle", "tribal" or "kindred", plus "permanent" where Oracle uses it type-like ("permanent card", "nonland permanent").</summary>
public enum CardType
{
    Permanent,

    Artifact,

    Creature,

    Enchantment,

    Instant,

    Land,

    Planeswalker,

    Sorcery,

    Battle,

    [RegexPattern("tribal", "kindred")]
    Tribal
}
