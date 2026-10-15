namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A combat restriction: "can't attack", "can't be blocked except by walls", "can't be blocked except by artifact creatures and/or black creatures", "can't attack unless defending player controls an island", "can't be blocked this turn", "can't block creatures with power 2 or greater".</summary>
/// <exampledoc>Juggernaut</exampledoc>
/// <examplecapture>can't be blocked by walls</examplecapture>
[Dependent]
public class CombatRestriction : Glyph, IPredicate
{
    public override Nib[] Nibs => ["can't", Prop(Action), Prop(Duration), Prop(Except), Prop(By), Prop(AndOr), Prop(Blocked), Prop(Condition)];

    public CombatAction Action { get; set; }
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
    public ControlCondition Condition { get; set; }
}
