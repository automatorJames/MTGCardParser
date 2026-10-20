namespace MTGGlyphs.GlyphDefinitions;

public enum LoyaltySign
{
    [RegexPattern(@"\+")]
    Plus,

    [RegexPattern("[−-]")]
    Minus
}
