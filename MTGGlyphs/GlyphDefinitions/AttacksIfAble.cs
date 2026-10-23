namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A forced attack or block, as a predicate: "attacks each combat if able", "blocks each combat if able", "attacks or blocks each combat if able", "attack this turn if able", or with no period, when something else attacks: "if this creature attacks, all creatures you control attack if able". Replaces the {this}-only AttacksEachCombat.</summary>
/// <exampledoc>Akoum Firebird</exampledoc>
/// <examplecapture>attacks each combat if able</examplecapture>
[Dependent]
public class AttacksIfAble : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Plural(Prop(Action)), Prop(OrBlocks), Prop(Period), "if able"];

    public CombatAction Action { get; set; }
    [RegexPattern("or blocks?")]
    public bool OrBlocks { get; set; }
    [Optional]
    public CombatPeriod? Period { get; set; }
}
