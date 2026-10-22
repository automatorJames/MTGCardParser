namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Paying a cost repeatedly, as the adversaries and their kin let you: "pay {1}{w} any number of times", "pay {g} up to three times". A predicate, so "you may pay {2}{r} any number of times" resolves; what the payments buy follows in a reflexive trigger (<see cref="PaidThisCostTrigger"/>).</summary>
/// <exampledoc>Intrepid Adversary</exampledoc>
/// <examplecapture>pay {1}{w} any number of times</examplecapture>
public class PayCostRepeatedly : Glyph, IPredicate
{
    public override Nib[] Nibs => ["pay", Prop(Cost), Prop(Times)];

    public Cost Cost { get; set; }
    public PaymentRepetition Times { get; set; }
}
