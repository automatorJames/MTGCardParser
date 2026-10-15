namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Damage divided among several recipients: "deals 4 damage divided as you choose among any number of targets", "deals x damage divided evenly, rounded down, among any number of targets". The division is announced as the spell is cast.</summary>
/// <exampledoc>Pyrotechnics</exampledoc>
/// <examplecapture>deals 4 damage divided as you choose among any number of targets</examplecapture>
[Dependent]
public class DealDividedDamage : Glyph, IPredicate
{
    public override Nib[] Nibs => [Pattern("deals?"), Prop(Amount), "damage divided", Prop(Division), "among any number of", Prop(Recipients)];

    public OneOf<ArithmeticAmount, Quantity?> Amount { get; set; }
    public DamageDivision Division { get; set; }
    public DividedRecipients Recipients { get; set; }
}
