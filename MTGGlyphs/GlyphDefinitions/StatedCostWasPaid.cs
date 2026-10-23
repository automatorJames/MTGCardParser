namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Condition naming an alternative cost by its mana: "the {2}{u} cost was paid", "that cost wasn't paid" (the Mastery cycle's "you may pay {2}{u} rather than pay this spell's mana cost").</summary>
/// <exampledoc>Ingenious Mastery</exampledoc>
/// <examplecapture>the {2}{u} cost was paid</examplecapture>
[Dependent]
public class StatedCostWasPaid : Glyph
{
    public override Nib[] Nibs => [Alt("the", "that"), Prop(Cost), "cost", Prop(Paid)];

    [Optional]
    public ManaCost Cost { get; set; }
    public CostPaidState Paid { get; set; }
}
