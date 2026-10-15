namespace MTGGlyphs.GlyphDefinitions;

public enum PayOptionType
{
    UnlessYou,

    UnlessThey,

    [RegexPattern("you may pay")]
    YouMay
}
