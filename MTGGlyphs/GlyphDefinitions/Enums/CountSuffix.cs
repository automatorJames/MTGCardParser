namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What may follow a count of targets before "target": "or more" ("one or more target creatures") or "other" ("up to two other target creatures"); one enum, since two optional properties in a row don't match.</summary>
public enum CountSuffix
{
    [RegexPattern("or more")]
    OrMore,

    Other
}
