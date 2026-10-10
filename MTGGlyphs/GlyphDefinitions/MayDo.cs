namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An optional action: "may draw a card" in "you may draw a card" - the subject chooses whether to do it.</summary>
/// <exampledoc>Verduran Enchantress</exampledoc>
/// <examplecapture>may draw a card</examplecapture>
[Dependent]
public class MayDo : Glyph, IPredicate
{
    public override Nib[] Nibs => ["may", Prop(Predicate)];

    [TypeFilter(typeof(IPredicate))]
    public DynamicGlyph Predicate { get; set; }
}
