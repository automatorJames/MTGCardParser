namespace MTGGlyphs.GlyphDefinitions;

public enum CostedAction
{
    [RegexPattern("to cast")]
    Cast,

    [RegexPattern("to activate")]
    Activate
}
