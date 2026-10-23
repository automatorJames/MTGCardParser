namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How a permanent enters the battlefield: "enters tapped", "enters tapped and attacking", "enters prepared", "enters with three +1/+1 counters on it" (modern Oracle wording drops "the battlefield"), "enters with your choice of a +1/+1 counter or haste" (riot), "escapes with a +1/+1 counter on it" (entering by escape), with something more as well ("enters with a +1/+1 counter on it and with "pay 3 life: regenerate this creature"", "enters with flying and with x +1/+1 counters on it"), for a period ("enter tapped this turn"), optionally under a condition: "enters tapped unless you control two or more other lands", "enters with a +1/+1 counter on it if you attacked this turn", "enters tapped unless a player has 13 or less life".</summary>
/// <exampledoc>Tetravus</exampledoc>
/// <examplecapture>enters with three +1/+1 counters on it</examplecapture>
[Dependent]
public class EntersTheBattlefield : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Way), Prop(State), Prop(Prepared), Prop(With), Prop(AlsoWith), Prop(Duration), Prop(Condition)];

    public EnteringWay Way { get; set; }
    [Optional]
    public EntryState? State { get; set; }
    [RegexPattern("prepared")]
    public bool Prepared { get; set; }
    [Optional]
    public OneOf<EntersWithCounters, RiotChoice, EntersWithAbility> With { get; set; }
    [Optional]
    public AndEntersWith AlsoWith { get; set; }
    [Optional]
    public ThisPeriod Duration { get; set; }
    [Optional]
    public OneOf<ControlCondition, IfHappenedThisTurn, StateCondition> Condition { get; set; }
}
