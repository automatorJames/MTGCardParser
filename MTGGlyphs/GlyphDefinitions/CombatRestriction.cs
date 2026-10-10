namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A combat restriction: "can't attack", "can't be blocked except by walls", "can't attack unless defending player controls an island", "can't be blocked this turn".</summary>
/// <exampledoc>Juggernaut</exampledoc>
/// <examplecapture>can't be blocked by walls</examplecapture>
[Dependent]
public class CombatRestriction : Glyph, IPredicate
{
    public override Nib[] Nibs => ["can't", Prop(Action), Prop(Duration), Prop(Except), Prop(By), Prop(Condition)];

    public CombatAction Action { get; set; }
    [Optional]
    public ThisPeriod Duration { get; set; }
    [RegexPattern("except")]
    public bool Except { get; set; }
    [Optional]
    public CombatantsBy By { get; set; }
    [Optional]
    public ControlCondition Condition { get; set; }
}
