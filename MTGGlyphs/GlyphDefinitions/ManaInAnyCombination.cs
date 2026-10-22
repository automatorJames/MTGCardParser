namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An amount of mana split among colors as the player likes: "two mana in any combination of colors", "x mana in any combination of {r} and/or {g}".</summary>
/// <exampledoc>Dawn's Reflection</exampledoc>
/// <examplecapture>two mana in any combination of colors</examplecapture>
[Dependent]
public class ManaInAnyCombination : Glyph
{
    public override Nib[] Nibs => [Prop(Quantity), "mana in any combination of", Prop(Of)];

    public Quantity Quantity { get; set; }
    public OneOf<ManaAndOr, ManyOf<ManaCost>, AnyCombinationOfColors?> Of { get; set; }
}
