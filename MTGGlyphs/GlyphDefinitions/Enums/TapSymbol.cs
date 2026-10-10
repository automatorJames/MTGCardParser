namespace MTGGlyphs.GlyphDefinitions;

public enum TapSymbol
{
    [RegexPattern(@"\{t\}")]
    Tap,

    [RegexPattern(@"\{q\}")]
    Untap,
}
