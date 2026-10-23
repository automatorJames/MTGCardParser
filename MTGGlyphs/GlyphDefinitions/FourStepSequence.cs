namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Four effects done in order, after one subject: "each player loses 1 life, discards a card, sacrifices a creature of their choice, then sacrifices a land of their choice", "that player loses 3 life, searches their library for a card, puts it into their hand, then shuffles". The later steps are predicates sharing the first step's subject. Like <see cref="ThreeStepSequence"/>, tried after other glyphs.</summary>
/// <exampledoc>Smallpox</exampledoc>
/// <examplecapture>each player loses 1 life, discards a card, sacrifices a creature of their choice, then sacrifices a land of their choice</examplecapture>
[TokenizationOrder(-2)]
public class FourStepSequence : Glyph
{
    public override Nib[] Nibs => [Prop(First), ",", Prop(Second), ",", Prop(Third), ", then", Prop(Next)];

    public SubjectDoes First { get; set; }
    [TypeFilter(typeof(IPredicate))]
    public DynamicGlyph Second { get; set; }
    [TypeFilter(typeof(IPredicate))]
    public DynamicGlyph Third { get; set; }
    [TypeFilter(typeof(IPredicate))]
    public DynamicGlyph Next { get; set; }
}
