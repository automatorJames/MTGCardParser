namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The extreme an effect picks by, "greatest" or "highest" and "least" or "lowest": "with the greatest power", "tied for lowest life total", "with the least toughness among creatures you control".</summary>
public enum Superlative
{
    [RegexPattern("greatest", "highest")]
    Greatest,

    [RegexPattern("least", "lowest")]
    Least
}
