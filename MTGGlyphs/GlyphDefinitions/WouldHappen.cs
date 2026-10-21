namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An event about to happen, as a replacement effect's condition: "you would draw a card", "a source would deal damage to you", "this creature would deal combat damage to a player". The subject and what it would do are slots.</summary>
/// <exampledoc>Alhammarret's Archive</exampledoc>
/// <examplecapture>you would draw a card</examplecapture>
[Dependent]
public class WouldHappen : Glyph
{
    public override Nib[] Nibs => [Prop(Subject), "would", Prop(Predicate)];

    public OneOf<Subject, ASource> Subject { get; set; }
    [TypeFilter(typeof(IPredicate))]
    public DynamicGlyph Predicate { get; set; }
}
