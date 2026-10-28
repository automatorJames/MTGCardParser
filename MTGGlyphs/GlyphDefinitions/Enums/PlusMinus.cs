namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The sign on each half of a power/toughness change: "+" or "-" in "+1/+1", "-1/-0".</summary>
public enum PlusMinus
{
    [RegexPattern(@"\+")]
    Plus,

    [RegexPattern("[−-]")]
    Minus,
}
