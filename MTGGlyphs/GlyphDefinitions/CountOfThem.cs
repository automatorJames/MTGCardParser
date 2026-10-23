namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A number of the cards or creatures just named: "one of them", "two of those cards", "any number of them", "up to one of them", "one of those creatures".</summary>
/// <exampledoc>Impulse</exampledoc>
/// <examplecapture>one of them</examplecapture>
[Dependent]
public class CountOfThem : Glyph
{
    public override Nib[] Nibs => [Opt("up to"), Prop(Quantity), "of", Alt("them", "those cards", "those creatures")];

    public Quantity Quantity { get; set; }
}
