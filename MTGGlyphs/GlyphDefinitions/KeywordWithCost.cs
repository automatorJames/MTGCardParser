namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A keyword with a mana cost parameter: "cycling {2}", "equip {1}", "flashback {3}{r}".</summary>
/// <exampledoc>Angel of the God-Pharaoh</exampledoc>
/// <examplecapture>cycling {2}</examplecapture>
[Dependent]
public class KeywordWithCost : Glyph
{
    public Keyword Keyword { get; set; }
    public ManaCost Cost { get; set; }
}
