namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The rider on a token copy that changes whether it is legendary: "except it isn't legendary", "except it's legendary".</summary>
public enum LegendaryChange
{
    [RegexPattern("except (it|the token) (isn't|is not) legendary", "except it's not legendary")]
    NotLegendary,

    [RegexPattern("except it's legendary")]
    Legendary
}
