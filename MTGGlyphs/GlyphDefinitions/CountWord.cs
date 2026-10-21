namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A number word followed by its space, for slots in glyphs joined without spaces (EnergyNote).</summary>
/// <exampledoc>Aether Chaser</exampledoc>
/// <examplecapture>two</examplecapture>
[Dependent]
public class CountWord : Glyph
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => [Prop(Quantity), " "];

    public Quantity Quantity { get; set; }
}
