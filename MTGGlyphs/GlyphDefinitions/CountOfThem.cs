namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A number of the cards just looked at: "one of them", "two of those cards", "any number of them", "up to one of them".</summary>
/// <exampledoc>Impulse</exampledoc>
/// <examplecapture>one of them</examplecapture>
[Dependent]
public class CountOfThem : Glyph
{
    public override Nib[] Nibs => [Opt("up to"), Prop(Quantity), "of", Alt("them", "those cards")];

    public Quantity Quantity { get; set; }
}
