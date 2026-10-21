namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Permanents that died: "creature that died this turn", "nontoken creature that died under your control this turn", "zubera that died this turn", "creature that died this way".</summary>
/// <exampledoc>Grizzly Ghoul</exampledoc>
/// <examplecapture>creature that died this turn</examplecapture>
[Dependent]
public class CreaturesThatDied : Glyph
{
    public override Nib[] Nibs => [Prop(Qualifiers), Prop(Kind), "that died", Prop(Under), Prop(When)];

    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }
    public PermanentKind Kind { get; set; }
    [Optional]
    public UnderControlOf Under { get; set; }
    public DeathScope When { get; set; }
}
