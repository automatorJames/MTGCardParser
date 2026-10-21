namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Two alternative things one subject may do, joined by "or": "cast it without paying its mana cost or put it into your hand", "sacrifice a nonland permanent of their choice or discard a card". The first is any predicate; the second one of the kinds marked <see cref="IAlternativePredicate"/>.</summary>
/// <exampledoc>Chimil, the Inner Sun</exampledoc>
/// <examplecapture>cast it without paying its mana cost or put it into your hand</examplecapture>
[Dependent]
public class PredicateEither : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(First), "or", Prop(Second)];

    [TypeFilter(typeof(IPredicate))]
    public DynamicGlyph First { get; set; }
    [TypeFilter(typeof(IAlternativePredicate))]
    public DynamicGlyph Second { get; set; }
}
