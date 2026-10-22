namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A static change to what spells or abilities cost: "this spell costs {1} less to cast for each creature card in your graveyard", "this ability costs {1} less to activate for each page counter on this artifact", "instant and sorcery spells you cast cost {1} less to cast", "white spells and black spells you cast cost {1} less to cast", "spells you cast from your graveyard cost {1} less to cast", "noncreature spells cost {1} more to cast", "this spell costs {1} less to cast if you control a wizard", "activated abilities of creatures you control cost {1} less to activate", "this spell costs {2} less to cast if it targets a tapped creature", "the second spell you cast each turn costs {1} less to cast". Affinity's reminder is one of these.</summary>
/// <exampledoc>Baral, Chief of Compliance</exampledoc>
/// <examplecapture>instant and sorcery spells you cast cost {1} less to cast</examplecapture>
public class CostModification : Glyph
{
    public override Nib[] Nibs => [Prop(Spells), Pattern("costs?"), Prop(Amount), Prop(Change), Alt("to cast", "to activate"), Prop(PerEach), Prop(Condition)];

    public OneOf<OneOf<ThisSpell, OrdinalSpellCast, ThisAbility>, This, OneOf<SpellKindsCast, SpellsCast, ActivatedAbilitiesOfGroup>> Spells { get; set; }
    public ManaCost Amount { get; set; }
    public CostChange Change { get; set; }
    [Optional]
    public ForEach PerEach { get; set; }
    [Optional]
    public OneOf<ControlCondition, IfHappenedThisTurn, OneOf<StateCondition, IfItTargets>> Condition { get; set; }
}
