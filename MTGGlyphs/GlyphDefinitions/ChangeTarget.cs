namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Redirecting a spell or ability: "change the target of target spell with a single target", "you may change the targets of target instant or sorcery spell", "change a target of target spell or ability to this creature".</summary>
/// <exampledoc>Deflection</exampledoc>
/// <examplecapture>change the target of target spell with a single target</examplecapture>
public class ChangeTarget : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["change", Alt("the target", "a target", "the targets"), "of", Prop(Retargeted), Prop(Restriction), Prop(NewTarget)];

    public OneOf<TargetAbility, TargetSpell> Retargeted { get; set; }
    [Optional]
    public TargetsRestriction Restriction { get; set; }
    [Optional]
    public NewTargetIs NewTarget { get; set; }
}
