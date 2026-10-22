namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// A subject and what it does or has: "target player draws three cards", "enchanted creature has flying", "you gain 1
/// life", "{this} deals 1 damage to that player" - or, adding to an earlier effect, also does: "that creature also gains trample until end of turn", "{this} also deals 2 damage to that creature's controller".
/// </summary>
/// <exampledoc>Ancestral Recall</exampledoc>
/// <examplecapture>target player draws three cards</examplecapture>
public class SubjectDoes : Glyph
{
    public override Nib[] Nibs => [Prop(Subject), Prop(Also), Prop(Predicate)];

    public Subject Subject { get; set; }
    [Optional]
    [RegexPattern("also")]
    public bool Also { get; set; }
    [TypeFilter(typeof(IPredicate))]
    public DynamicGlyph Predicate { get; set; }
}
