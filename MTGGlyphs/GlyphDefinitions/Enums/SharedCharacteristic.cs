namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The characteristic one object matches another in, after "has the same": "name" ("it has the same name as that card") or "mana value" ("it has the same mana value as the revealed card").</summary>
public enum SharedCharacteristic
{
    Name,

    [RegexPattern("mana value")]
    ManaValue
}
