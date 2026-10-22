namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Creating a Role token (an aura token) attached to a creature: "create a wicked role token attached to up to one target creature you control", "create a cursed role token attached to that creature".</summary>
/// <exampledoc>Witch's Mark</exampledoc>
/// <examplecapture>create a wicked role token attached to up to one target creature you control</examplecapture>
public class CreateRoleToken : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["create a", Prop(Role), "role token attached to", Prop(AttachedTo)];

    public RoleKind Role { get; set; }
    public OneOf<CountedTargets, PermanentPhrase> AttachedTo { get; set; }
}
