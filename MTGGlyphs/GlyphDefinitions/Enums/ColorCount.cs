namespace MTGGlyphs.GlyphDefinitions;

public enum ColorCount
{
    [RegexPattern("one or more colors")]
    OneOrMoreColors,

    Colorless,

    Monocolored,

    Multicolored
}
