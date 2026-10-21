namespace MTGGlyphs.GlyphDefinitions;

public enum RevealOrExile
{
    [RegexPattern("reveals?")]
    Reveal,

    [RegexPattern("exiles?")]
    Exile
}
