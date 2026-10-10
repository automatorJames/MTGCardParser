namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// "target creature", "target nonartifact, nonblack creature", "target creature with flying", "target creature with power 2 or less", "target player", "target artifact, creature, or land": "target" and what it targets - one kind, or a choice of kinds. What
/// it targets is a referent, so a later "it" refers to it - "destroy target wall. it can't be regenerated." - and so
/// does a later "that creature", "that wall" or "that player", whose kind (PlayerIdentity, CardType, CreatureType) is
/// the type of what was captured.
/// </summary>
/// <exampledoc>Crumble</exampledoc>
/// <examplecapture>target artifact</examplecapture>
[Dependent]
public class SpecificTarget : Glyph
{
    public override Nib[] Nibs => ["target", Prop(Qualifiers), Prop(Kind), Prop(With)];

    [Optional]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }
    [Referent]
    [Singular]
    public OneOf<ManyOf<TargetKind>, TargetKind> Kind { get; set; }
    [Optional]
    public WithQualifier With { get; set; }
}
