namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The two ways to pay a twobrid symbol: "any two mana or with {u}".</summary>
/// <exampledoc>Spectral Procession</exampledoc>
/// <examplecapture>any two mana or with {w}</examplecapture>
[Dependent]
public class TwobridPayment : Glyph
{
    public override Nib[] Nibs => [Prop(First), "or with", Prop(Second)];

    public ManaPaymentOption First { get; set; }
    public ManaPaymentOption Second { get; set; }
}
