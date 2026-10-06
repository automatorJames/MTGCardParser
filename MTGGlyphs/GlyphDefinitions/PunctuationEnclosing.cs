namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A quotation mark around a quoted ability, tokenized last so the quoted text is matched first.</summary>
/// <exampledoc>Farmstead</exampledoc>
/// <examplecapture>"</examplecapture>
[TokenizationOrder(9999)]
public class PunctuationEnclosing : Glyph
{
    public EnclosingPunctuationCharacter EnclosingPunctuationCharacter { get; set; }
}

public enum EnclosingPunctuationCharacter
{
    [RegexPattern(@"""")]
    DoubleQuote,
}