namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The verb that animates or transforms a permanent, with its article when singular: "becomes a 4/4 red and green dragon artifact creature", "becomes an artifact creature", "become 3/3 elemental creatures with haste".</summary>
public enum AnimateVerb
{
    [RegexPattern("becomes? an?", "become")]
    Becomes
}
