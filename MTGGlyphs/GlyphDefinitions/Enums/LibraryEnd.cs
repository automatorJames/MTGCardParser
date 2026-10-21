namespace MTGGlyphs.GlyphDefinitions;

public enum LibraryEnd
{
    [RegexPattern("the bottom")]
    Bottom,

    Top,

    [RegexPattern("(their|your) choice of the top or bottom")]
    TopOrBottom
}
