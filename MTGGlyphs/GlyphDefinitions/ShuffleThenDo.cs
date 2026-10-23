namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"shuffle, then discard a card at random": a library search that shuffles and then has one more step.</summary>
/// <exampledoc>Curse-Marred Demon</exampledoc>
/// <examplecapture>shuffle, then discard a card at random</examplecapture>
[Dependent]
public class ShuffleThenDo : Glyph
{
    public override Nib[] Nibs => [Alt("shuffle", "shuffles"), ", then", Prop(Action)];

    [TypeFilter(typeof(IPredicate))]
    public DynamicGlyph Action { get; set; }
}
