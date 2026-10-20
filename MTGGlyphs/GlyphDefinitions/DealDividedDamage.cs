namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Damage divided among several recipients: "deals 4 damage divided as you choose among any number of targets", "deals x damage divided evenly, rounded down, among any number of targets", "deals 3 damage divided as you choose among one, two, or three targets", "… among one or two target creatures with flying". The division is announced as the spell is cast.</summary>
/// <exampledoc>Pyrotechnics</exampledoc>
/// <examplecapture>deals 4 damage divided as you choose among any number of targets</examplecapture>
[Dependent]
public class DealDividedDamage : Glyph, IPredicate
{
    public override Nib[] Nibs => [Pattern("deals?"), Prop(Amount), "damage divided", Prop(Division), "among", Prop(Recipients)];

    public OneOf<ArithmeticAmount, Quantity?> Amount { get; set; }
    public DamageDivision Division { get; set; }
    public OneOf<AnyNumberOfRecipients, ChosenNumberOfTargets> Recipients { get; set; }
}
