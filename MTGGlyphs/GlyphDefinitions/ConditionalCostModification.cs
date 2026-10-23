namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A cost change that applies only under a condition CostModification's typed conditions don't cover: "this spell costs {2} less to cast if evidence was collected", "this spell costs {2} less to cast if it's night", "those spells cost {r} less to cast if you paid life this way". The condition resolves to any condition glyph; tried after CostModification, so its lines keep their parse.</summary>
/// <exampledoc>Moonrager's Slash</exampledoc>
/// <examplecapture>this spell costs {2} less to cast if it's night</examplecapture>
[TokenizationOrder(-1)]
public class ConditionalCostModification : Glyph
{
    public override Nib[] Nibs => [Prop(Spells), Pattern("costs?"), Prop(Amount), Prop(Change), Alt("to cast", "to activate"), "if", Prop(Condition)];

    public OneOf<ThisSpell, ThisAbility, OneOf<This, ThoseSpells>> Spells { get; set; }
    public ManaCost Amount { get; set; }
    public CostChange Change { get; set; }
    public DynamicGlyph Condition { get; set; }
}
