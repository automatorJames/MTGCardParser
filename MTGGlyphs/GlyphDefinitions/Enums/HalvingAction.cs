namespace MTGGlyphs.GlyphDefinitions;

public enum HalvingAction
{
    [RegexPattern("mills?")]
    Mill,

    [RegexPattern("discards?")]
    Discard,

    [RegexPattern("sacrifices?")]
    Sacrifice
}
