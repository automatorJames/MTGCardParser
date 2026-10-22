namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// "target creature", "another target creature", "up to one target creature", "up to one other target creature", "target attacking or blocking creature", "target nonartifact, nonblack creature", "target tapped nonblack creature", "target unblocked attacking creature" (space-joined qualifiers through QualifierWords), "target creature with flying", "target creature with power 2 or less", "target creature you control", "target artifact defending player controls", "target creature with flying an opponent controls" (the controller after the qualifier), "target player", "target artifact, creature, or land", "target red creature or red planeswalker" (each kind qualified on its own), "target creature that was dealt damage this turn", "target creature without flying that's attacking you", "target creature that entered this turn", "target creature that dealt damage to you this turn", "target aura attached to a creature", "target creature other than this creature", "target permanent that's one or more colors", "target creature of defending player's choice", "target nonartifact creature that player controls of their choice": "target" and what it targets - one kind, or a choice of kinds, optionally by who controls it and who chooses it. What
/// it targets is a referent, so a later "it" refers to it - "destroy target wall. it can't be regenerated." - and so
/// does a later "that creature", "that wall" or "that player", whose kind (PlayerIdentity, CardType, CreatureType) is
/// the type of what was captured. The prefix is one optional enum: two leading bools in a row don't match. "up to one target …" is a single (optional) target, so it's here; larger counts are CountedTargets.
/// </summary>
/// <exampledoc>Crumble</exampledoc>
/// <examplecapture>target artifact</examplecapture>
[Dependent]
public class SpecificTarget : Glyph
{
    public override Nib[] Nibs => [Prop(Prefix), "target", Prop(Qualifiers), Prop(Kind), Prop(Controller), Prop(OtherThan), Prop(ChosenBy), Prop(With), Prop(ControllerAfterWith), Prop(InCombat), Prop(DealtDamageThisTurn)];

    [Optional]
    public TargetPrefix? Prefix { get; set; }
    [Optional]
    public OneOf<ManyOf<TargetQualifier>, CompoundOf<TargetQualifier>, QualifierWords> Qualifiers { get; set; }
    [Referent]
    [Singular]
    public OneOf<ManyOf<TargetKind>, TargetKind, ManyOf<QualifiedTargetKind>> Kind { get; set; }
    [Optional]
    public ControlledBy Controller { get; set; }
    [Optional]
    public OtherThanThis OtherThan { get; set; }
    [Optional]
    public OfPlayersChoice ChosenBy { get; set; }
    [Optional]
    public WithQualifier With { get; set; }
    [Optional]
    public ControlledBy ControllerAfterWith { get; set; }
    [Optional]
    public OneOf<ThatsInCombat, EnteredThisTurn, OneOf<ThatDidThisTurn, AttachedToObject, ThatsColored>> InCombat { get; set; }
    [RegexPattern("that was dealt damage this turn")]
    public bool DealtDamageThisTurn { get; set; }
}
