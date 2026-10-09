namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A combat restriction: "can't attack", "can't be blocked except by walls", "can't attack unless defending player controls an island".</summary>
/// <exampledoc>Juggernaut</exampledoc>
/// <examplecapture>can't be blocked by walls</examplecapture>
[Dependent]
public class CombatRestriction : Glyph, IPredicate
{
    public override Nib[] Nibs => ["can't", Prop(Action), Prop(Except), Prop(By), Prop(Condition)];

    public CombatAction Action { get; set; }
    [RegexPattern("except")]
    public bool Except { get; set; }
    [Optional]
    public CombatantsBy By { get; set; }
    [Optional]
    public ControlCondition Condition { get; set; }
}
