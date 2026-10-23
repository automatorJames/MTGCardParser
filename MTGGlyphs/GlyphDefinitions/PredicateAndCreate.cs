namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An action followed by "and create …" tokens: "you lose 1 life and create a 1/1 blue and black faerie creature token". A typed twin of <see cref="PredicatePair"/>, whose open first slot ends at the last "and" of the clause and so can't get past a token's color list ("blue and black").</summary>
/// <exampledoc>Bitterbloom Bearer</exampledoc>
/// <examplecapture>lose 1 life and create a 1/1 blue and black faerie creature token with flying</examplecapture>
[Dependent]
public class PredicateAndCreate : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(First), "and", Prop(Second)];

    [TypeFilter(typeof(IPredicate))]
    public DynamicGlyph First { get; set; }
    public CreateToken Second { get; set; }
}
