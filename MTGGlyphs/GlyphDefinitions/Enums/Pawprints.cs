namespace MTGGlyphs.GlyphDefinitions;

public enum Pawprints
{
    [RegexPattern(@"\{p\}\{p\}\{p\}")]
    Three,

    [RegexPattern(@"\{p\}\{p\}")]
    Two,

    [RegexPattern(@"\{p\}")]
    One
}
