namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A keyword ability on its own, e.g. "flying".</summary>
/// <exampledoc>Air Elemental</exampledoc>
/// <examplecapture>flying</examplecapture>
[Dependent]
public class CardKeyword : Glyph
{
    public Keyword Keyword { get; set; }
}