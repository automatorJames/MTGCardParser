namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Permanents that died: "creature that died this turn", "nontoken creature that died under your control this turn", "zubera that died this turn", "creature that died this way", "creature put into your graveyard from the battlefield this turn".</summary>
/// <exampledoc>Grizzly Ghoul</exampledoc>
/// <examplecapture>creature that died this turn</examplecapture>
[Dependent]
public class CreaturesThatDied : Glyph
{
    public override Nib[] Nibs => [Prop(Qualifiers), Prop(Kind), Prop(Died), Prop(Under), Prop(When)];

    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }
    public PermanentKind Kind { get; set; }
    public DiedWording Died { get; set; }
    [Optional]
    public UnderControlOf Under { get; set; }
    public DeathScope When { get; set; }
}
