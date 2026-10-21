namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How a permanent enters the battlefield: "enters tapped", "enters prepared", "enters with three +1/+1 counters on it" (modern Oracle wording drops "the battlefield"), "enters with your choice of a +1/+1 counter or haste" (riot), "escapes with a +1/+1 counter on it" (entering by escape), optionally under a condition: "enters tapped unless you control two or more other lands", "enters with a +1/+1 counter on it if you attacked this turn", "enters tapped unless a player has 13 or less life".</summary>
/// <exampledoc>Tetravus</exampledoc>
/// <examplecapture>enters with three +1/+1 counters on it</examplecapture>
[Dependent]
public class EntersTheBattlefield : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Way), Prop(Tapped), Prop(Prepared), Prop(With), Prop(Condition)];

    public EnteringWay Way { get; set; }
    [RegexPattern("tapped")]
    public bool Tapped { get; set; }
    [RegexPattern("prepared")]
    public bool Prepared { get; set; }
    [Optional]
    public OneOf<EntersWithCounters, RiotChoice> With { get; set; }
    [Optional]
    public OneOf<ControlCondition, IfHappenedThisTurn, StateCondition> Condition { get; set; }
}
