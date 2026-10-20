namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A static change to what spells cost: "this spell costs {1} less to cast for each creature card in your graveyard", "instant and sorcery spells you cast cost {1} less to cast", "noncreature spells cost {1} more to cast", "this spell costs {1} less to cast if you control a wizard", "this spell costs {3} less to cast if a creature died this turn". Affinity's reminder is one of these.</summary>
/// <exampledoc>Baral, Chief of Compliance</exampledoc>
/// <examplecapture>instant and sorcery spells you cast cost {1} less to cast</examplecapture>
public class CostModification : Glyph
{
    public override Nib[] Nibs => [Prop(Spells), Pattern("costs?"), Prop(Amount), Prop(Change), "to cast", Prop(PerEach), Prop(Condition)];

    public OneOf<ThisSpell, This, SpellsCast> Spells { get; set; }
    public ManaCost Amount { get; set; }
    public CostChange Change { get; set; }
    [Optional]
    public ForEach PerEach { get; set; }
    [Optional]
    public OneOf<ControlCondition, IfHappenedThisTurn> Condition { get; set; }
}
