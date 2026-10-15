namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Colorless mana's reminder text.</summary>
/// <exampledoc>Eldrazi Temple</exampledoc>
/// <examplecapture>{c} represents colorless mana</examplecapture>
public class ColorlessManaRule : Glyph
{
    public override Nib[] Nibs => ["{c} represents colorless mana"];
}
