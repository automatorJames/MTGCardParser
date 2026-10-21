namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A cost that may go unpaid, possibly multiplied: "unless you pay {u}", "you may pay 1 life", "unless you pay {g} for each wind counter on it", "unless its controller pays {1} for each card in your graveyard", "unless any player pays {3}", "unless you sacrifice two islands", "unless you discard a card", "unless you mill two cards", "unless they sacrifice a nonland permanent of their choice or discard a card". After "unless", the paying verb is left out when the cost carries its own ("sacrifice", "discard", "tap").</summary>
/// <exampledoc>Phantasmal Forces</exampledoc>
/// <examplecapture>unless you pay {u}</examplecapture>
[Dependent]
public class OptionalPayCost : Glyph
{
    public override Nib[] Nibs => [Prop(PayOptionType), Opt(Pattern("pays?")), Prop(Cost), Prop(PerEach)];

    public OneOf<PayOptionType?, UnlessPlayer> PayOptionType { get; set; }
    public OneOf<CostEither, Cost> Cost { get; set; }
    [Optional]
    public OneOf<ForEachCounter, ForEach> PerEach { get; set; }
}
