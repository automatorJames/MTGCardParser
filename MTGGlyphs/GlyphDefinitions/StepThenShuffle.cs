namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"attach it to a creature you control, then shuffle": a library search with one more step after placing the card, before the shuffle.</summary>
/// <exampledoc>Stonehewer Giant</exampledoc>
/// <examplecapture>attach it to a creature you control, then shuffle</examplecapture>
[Dependent]
public class StepThenShuffle : Glyph
{
    public override Nib[] Nibs => [Prop(Step), ", then", Alt("shuffle", "shuffles")];

    [TypeFilter(typeof(IPredicate))]
    public DynamicGlyph Step { get; set; }
}
