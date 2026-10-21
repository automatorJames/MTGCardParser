namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Paying a cost and then doing something, as one action: "pay {w} and exile it with three time counters on it" (suspend), "pay {2}{g} and exile this card from your hand" (plot). A lone "you may pay {1}" stays an <see cref="OptionalPayCost"/>.</summary>
/// <exampledoc>Rift Bolt</exampledoc>
/// <examplecapture>pay {r} and exile it with a time counter on it</examplecapture>
[Dependent]
public class PayAndDo : Glyph, IPredicate
{
    public override Nib[] Nibs => [Pattern("pays?"), Prop(Cost), "and", Prop(Then)];

    public Cost Cost { get; set; }
    [TypeFilter(typeof(IConjoinedPredicate))]
    public DynamicGlyph Then { get; set; }
}
