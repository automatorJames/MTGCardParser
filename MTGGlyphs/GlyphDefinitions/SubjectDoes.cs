namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// A subject and what it does or has: "target player draws three cards", "enchanted creature has flying", "you gain 1
/// life", "{this} deals 1 damage to that player".
/// </summary>
/// <exampledoc>Ancestral Recall</exampledoc>
/// <examplecapture>target player draws three cards</examplecapture>
public class SubjectDoes : Glyph
{
    public override Nib[] Nibs => [Prop(Subject), Prop(Predicate)];

    public Subject Subject { get; set; }

    [TypeFilter(typeof(IPredicate))]
    public DynamicGlyph Predicate { get; set; }
}
