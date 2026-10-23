namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// A subject and what it does or has: "target player draws three cards", "enchanted creature has flying", "you gain 1
/// life", "{this} deals 1 damage to that player" - or, adding to an earlier effect, also does: "that creature also gains trample until end of turn", "{this} also deals 2 damage to that creature's controller" - or, for a group, each does on their own: "any number of target players each gain 6 life", "up to two target creatures each get +1/+1", "they each deal damage equal to their power to …".
/// </summary>
/// <exampledoc>Ancestral Recall</exampledoc>
/// <examplecapture>target player draws three cards</examplecapture>
public class SubjectDoes : Glyph
{
    public override Nib[] Nibs => [Prop(Subject), Prop(Also), Prop(Each), Prop(Predicate)];

    public Subject Subject { get; set; }
    [Optional]
    [RegexPattern("also")]
    public bool Also { get; set; }
    [Optional]
    [RegexPattern("each")]
    public bool Each { get; set; }
    [TypeFilter(typeof(IPredicate))]
    public DynamicGlyph Predicate { get; set; }
}
