namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// "target creature", "another target creature", "up to one other target creature", "target attacking or blocking creature", "target nonartifact, nonblack creature", "target creature with flying", "target creature with power 2 or less", "target creature you control", "target artifact defending player controls", "target player", "target artifact, creature, or land": "target" and what it targets - one kind, or a choice of kinds, optionally by who controls it. What
/// it targets is a referent, so a later "it" refers to it - "destroy target wall. it can't be regenerated." - and so
/// does a later "that creature", "that wall" or "that player", whose kind (PlayerIdentity, CardType, CreatureType) is
/// the type of what was captured. The prefix is one optional enum: two leading bools in a row don't match. Plain "up to one target …" is CountedTargets.
/// </summary>
/// <exampledoc>Crumble</exampledoc>
/// <examplecapture>target artifact</examplecapture>
[Dependent]
public class SpecificTarget : Glyph
{
    public override Nib[] Nibs => [Prop(Prefix), "target", Prop(Qualifiers), Prop(Kind), Prop(Controller), Prop(With)];

    [Optional]
    public TargetPrefix? Prefix { get; set; }
    [Optional]
    public OneOf<ManyOf<TargetQualifier>, CompoundOf<TargetQualifier>> Qualifiers { get; set; }
    [Referent]
    [Singular]
    public OneOf<ManyOf<TargetKind>, TargetKind> Kind { get; set; }
    [Optional]
    public ControlledBy Controller { get; set; }
    [Optional]
    public WithQualifier With { get; set; }
}
