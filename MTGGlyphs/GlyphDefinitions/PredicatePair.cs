namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Two things one subject does, joined by "and": "{this} gets +2/+0 until end of turn and deals 1 damage to you". The first part is any predicate; the second one of the kinds marked <see cref="IConjoinedPredicate"/>.</summary>
/// <exampledoc>Electric Eel</exampledoc>
/// <examplecapture>gets +2/+0 until end of turn and deals 1 damage to you</examplecapture>
[Dependent]
public class PredicatePair : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(First), "and", Prop(Second)];

    [TypeFilter(typeof(IPredicate))]
    public DynamicGlyph First { get; set; }
    [TypeFilter(typeof(IConjoinedPredicate))]
    public DynamicGlyph Second { get; set; }
}
