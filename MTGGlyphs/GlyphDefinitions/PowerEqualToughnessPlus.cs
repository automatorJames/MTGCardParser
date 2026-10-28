namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The "goyf" static ability: power equal to an amount and toughness that amount plus a number: "{this}'s power is equal to the number of card types among cards in all graveyards and its toughness is equal to that number plus 1".</summary>
/// <exampledoc>Tarmogoyf</exampledoc>
/// <examplecapture>{this}'s power is equal to the number of card types among cards in all graveyards and its toughness is equal to that number plus 1</examplecapture>
public class PowerEqualToughnessPlus : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Object), "power is equal to", Prop(Amount), "and its toughness is equal to that number plus", Prop(ToughnessBonus)];

    public OneOf<Its, ObjectPossessive> Object { get; set; }
    public OneOf<ArithmeticAmount, ScaledAmountExact, Amount> Amount { get; set; }
    public Quantity ToughnessBonus { get; set; }
}
