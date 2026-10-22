namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A condition on the most common color among all permanents (the Djinn cycle's drawback): "black is the most common color among all permanents or is tied for most common".</summary>
/// <exampledoc>Goham Djinn</exampledoc>
/// <examplecapture>black is the most common color among all permanents or is tied for most common</examplecapture>
[Dependent]
public class ColorIsMostCommon : Glyph
{
    public override Nib[] Nibs => [Prop(Color), "is the most common color among all permanents or is tied for most common"];

    public ManaColor Color { get; set; }
}
