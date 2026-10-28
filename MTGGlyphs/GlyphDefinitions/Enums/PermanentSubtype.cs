namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A subtype that is neither a creature type nor a land type, used as a noun or qualifier: "saga", "room", "role", "background", "case", "adventure", "rune", "plan", "omen" ("target saga", "a role token").</summary>
public enum PermanentSubtype
{
    Saga,

    Room,

    Role,

    Background,

    Case,

    Adventure,

    [RegexPattern("runes?")]
    Rune,

    Plan,

    Omen
}
