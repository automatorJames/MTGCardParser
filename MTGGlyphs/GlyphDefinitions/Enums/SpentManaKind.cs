namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What's counted in "the amount of … spent to cast": "mana" or snow mana, "{s}" ("the amount of {s} spent to cast this spell").</summary>
public enum SpentManaKind
{
    Mana,

    [RegexPattern(@"\{s\}")]
    Snow
}
