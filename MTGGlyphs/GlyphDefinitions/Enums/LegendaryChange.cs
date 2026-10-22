namespace MTGGlyphs.GlyphDefinitions;

public enum LegendaryChange
{
    [RegexPattern("except (it|the token) (isn't|is not) legendary", "except it's not legendary")]
    NotLegendary,

    [RegexPattern("except it's legendary")]
    Legendary
}
