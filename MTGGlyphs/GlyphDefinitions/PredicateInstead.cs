namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A predicate replacing the usual one, "… instead", where a trailing condition follows: "deals 5 damage instead (if a creature died this turn)", "gain 10 life instead (if …)".</summary>
/// <exampledoc>Galvanic Blast</exampledoc>
/// <examplecapture>deals 4 damage instead</examplecapture>
[Dependent]
public class PredicateInstead : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Predicate), "instead"];

    [TypeFilter(typeof(IPredicate))]
    public DynamicGlyph Predicate { get; set; }
}
