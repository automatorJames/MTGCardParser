namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An open-ended range: the "+" of "4+", "or more".</summary>
/// <exampledoc>Brimstone Mage</exampledoc>
/// <examplecapture>+</examplecapture>
[Dependent]
public class OrMore : Glyph
{
    public override Nib[] Nibs => ["+"];
}
