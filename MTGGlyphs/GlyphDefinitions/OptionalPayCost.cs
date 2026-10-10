namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A cost that may go unpaid: "unless you pay {u}", "you may pay 1 life".</summary>
/// <exampledoc>Phantasmal Forces</exampledoc>
/// <examplecapture>unless you pay {u}</examplecapture>
[Dependent]
public class OptionalPayCost : Glyph
{
    public override Nib[] Nibs => [Prop(PayOptionType), "pay", Prop(Cost)];

    public PayOptionType PayOptionType { get; set; }
    public Cost Cost { get; set; }
}
