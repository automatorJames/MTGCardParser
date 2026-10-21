namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A combat restriction: "can't attack", "can't attack or block", "can't attack alone", "can't be blocked except by walls", "can't be blocked except by artifact creatures and/or black creatures", "can't attack unless defending player controls an island", "can't attack unless there are seven or more cards in your graveyard", "can't be blocked this turn", "can't block creatures with power 2 or greater". Also the second of two predicates: "can't block and can't be blocked", "gets +2/+2 and can't block".</summary>
/// <exampledoc>Juggernaut</exampledoc>
/// <examplecapture>can't be blocked by walls</examplecapture>
[Dependent]
public class CombatRestriction : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["can't", Prop(Action), Prop(OrAction), Prop(Alone), Prop(Duration), Prop(Except), Prop(By), Prop(AndOr), Prop(Blocked), Prop(Condition)];

    public CombatAction Action { get; set; }
    [Optional]
    public OrCombatAction OrAction { get; set; }
    [RegexPattern("alone")]
    public bool Alone { get; set; }
    [Optional]
    public ThisPeriod Duration { get; set; }
    [RegexPattern("except")]
    public bool Except { get; set; }
    [Optional]
    public CombatantsBy By { get; set; }
    [Optional]
    public AndOrKind AndOr { get; set; }
    [Optional]
    public BlockedKinds Blocked { get; set; }
    [Optional]
    public OneOf<ControlCondition, StateCondition> Condition { get; set; }
}
