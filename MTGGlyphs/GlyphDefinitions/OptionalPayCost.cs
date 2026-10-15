namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A cost that may go unpaid, possibly multiplied per counter: "unless you pay {u}", "you may pay 1 life", "unless you pay {g} for each wind counter on it", "unless its controller pays {1}", "unless any player pays {3}", "unless you sacrifice two islands", "unless you discard a card". After "unless", the paying verb is left out when the cost carries its own ("sacrifice", "discard", "tap").</summary>
/// <exampledoc>Phantasmal Forces</exampledoc>
/// <examplecapture>unless you pay {u}</examplecapture>
[Dependent]
public class OptionalPayCost : Glyph
{
    public override Nib[] Nibs => [Prop(PayOptionType), Opt(Pattern("pays?")), Prop(Cost), Prop(PerEach)];

    public OneOf<PayOptionType?, UnlessPlayer> PayOptionType { get; set; }
    public Cost Cost { get; set; }
    [Optional]
    public ForEachCounter PerEach { get; set; }
}
