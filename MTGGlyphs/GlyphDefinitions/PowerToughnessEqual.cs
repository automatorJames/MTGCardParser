namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A static ability setting an object's power and/or toughness to an amount: "{this}'s power and toughness are each equal to the number of swamps you control", "molimo's power and toughness are each equal to the number of lands you control", "… equal to 1 plus the number of creatures the chosen player controls", "… equal to twice the number of cards in your hand", "… equal to half the highest life total among your opponents, rounded up", "… equal to your life total".</summary>
/// <exampledoc>Nightmare</exampledoc>
/// <examplecapture>{this}'s power and toughness are each equal to the number of swamps you control</examplecapture>
public class PowerToughnessEqual : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Object), Prop(PowerAndOrToughness), Alt("are each", "is"), "equal to", Prop(Amount)];

    public OneOf<Its, ObjectPossessive> Object { get; set; }
    public PowerAndOrToughness PowerAndOrToughness { get; set; }
    public OneOf<ArithmeticAmount, ScaledAmount, ScaledAmountExact, Amount> Amount { get; set; }
}
