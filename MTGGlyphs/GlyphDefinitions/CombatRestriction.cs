namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A combat restriction: "can't attack", "can't attack or block", "can't attack, block, or crew vehicles", "can't attack alone", "can't attack you", "can't attack you or planeswalkers you control", "can't be blocked except by walls", "can't be blocked except by artifact creatures and/or black creatures", "can't attack unless defending player controls an island", "can't attack unless there are seven or more cards in your graveyard", "can't be blocked this turn", "can't attack or block until your next turn", "can't attack or block for as long as you control this vehicle", "can't block creatures with power 2 or greater", "can't block it", "can't block this creature this turn", "can't block creatures you control". Also the second of two predicates: "can't block and can't be blocked", "gets +2/+2 and can't block".</summary>
/// <exampledoc>Juggernaut</exampledoc>
/// <examplecapture>can't be blocked by walls</examplecapture>
[Dependent]
public class CombatRestriction : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["can't", Prop(Action), Prop(Attacked), Prop(OrAction), Prop(Alone), Prop(Duration), Prop(Except), Prop(By), Prop(AndOr), Prop(Blocked), Prop(Blockee), Prop(Condition)];

    public OneOf<CombatAction?, ManyOf<CombatAction?>> Action { get; set; }
    [Optional]
    public AttackRecipient? Attacked { get; set; }
    [Optional]
    public OrCombatAction OrAction { get; set; }
    [RegexPattern("alone")]
    public bool Alone { get; set; }
    [Optional]
    public OneOf<ThisPeriod, UntilPhase, ForAsLongAs> Duration { get; set; }
    [RegexPattern("except")]
    public bool Except { get; set; }
    [Optional]
    public CombatantsBy By { get; set; }
    [Optional]
    public AndOrKind AndOr { get; set; }
    [Optional]
    public BlockedKinds Blocked { get; set; }
    [Optional]
    public BlockedObject Blockee { get; set; }
    [Optional]
    public OneOf<ControlCondition, StateCondition> Condition { get; set; }
}
