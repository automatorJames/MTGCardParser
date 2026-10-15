namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A cost that may go unpaid, possibly multiplied per counter: "unless you pay {u}", "you may pay 1 life", "unless you pay {g} for each wind counter on it".</summary>
/// <exampledoc>Phantasmal Forces</exampledoc>
/// <examplecapture>unless you pay {u}</examplecapture>
[Dependent]
public class OptionalPayCost : Glyph
{
    public override Nib[] Nibs => [Prop(PayOptionType), "pay", Prop(Cost), Prop(PerEach)];

    public PayOptionType PayOptionType { get; set; }
    public Cost Cost { get; set; }
    [Optional]
    public ForEachCounter PerEach { get; set; }
}
