namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A keyword with a numeric parameter: "bushido 1", "renown 1", "annihilator 2", "modular 3".</summary>
/// <exampledoc>Akki Underminer</exampledoc>
/// <examplecapture>bushido 1</examplecapture>
[Dependent]
public class KeywordWithAmount : Glyph
{
    public Keyword Keyword { get; set; }
    public Quantity Amount { get; set; }
}
