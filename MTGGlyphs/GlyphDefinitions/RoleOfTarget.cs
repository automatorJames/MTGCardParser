namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The owner or controller of a target, named first, as a subject: "the owner of target nonland permanent", "the controller of target instant or sorcery spell", "the owner of target spell or creature" - the "of" form of <see cref="TargetsController"/>.</summary>
/// <exampledoc>Run Out of Town</exampledoc>
/// <examplecapture>the owner of target nonland permanent</examplecapture>
[Dependent]
public class RoleOfTarget : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Role), "of", Prop(Target)];

    public PlayerRole Role { get; set; }
    public OneOf<TargetSpellOrPermanent, SpecificTarget, OneOf<TargetCardInZone, TargetSpell>> Target { get; set; }
}
