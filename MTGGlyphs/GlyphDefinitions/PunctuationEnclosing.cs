namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(9999)]
[Color("#999999")]
public class PunctuationEnclosing : Glyph
{
    public EnclosingPunctuationCharacter EnclosingPunctuationCharacter { get; set; }
}

public enum EnclosingPunctuationCharacter
{
    [RegexPattern(@"""")]
    DoubleQuote,
}