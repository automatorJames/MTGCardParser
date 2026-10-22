namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"each creature you control", "each creature your opponents control with a +1/+1 counter on it", "each other creature you control that's a wolf or a werewolf": every permanent of a kind a player controls, one at a time, as a subject. Tried before EachObject, which has no controller and would otherwise claim "each creature" and leave "you control" unmatched.</summary>
/// <exampledoc>Armorcraft Judge</exampledoc>
/// <examplecapture>each creature you control with a +1/+1 counter on it</examplecapture>
[Dependent]
public class EachControlledPermanent : Glyph
{
    public override Nib[] Nibs => ["each", Prop(Qualifiers), Prop(Kind), Prop(Controller), Prop(Narrowed), Prop(After)];

    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }
    [Referent]
    [Singular]
    public PermanentKind Kind { get; set; }
    public ControlledBy Controller { get; set; }
    [Optional]
    public OneOf<WithQualifier, ChosenAspect?> Narrowed { get; set; }
    [Optional]
    public OneOf<ThatsInCombat, SharesCreatureType, OneOf<EnteredThisTurn, ThatsOneOfKinds>> After { get; set; }
}
