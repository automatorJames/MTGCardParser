namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Exiling a card face down, so it's hidden: "exile one face down" (hideaway), "exile it face down".</summary>
/// <exampledoc>Howltooth Hollow</exampledoc>
/// <examplecapture>exile one face down</examplecapture>
[Dependent]
public class ExileFaceDown : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["exile", Prop(Pick), "face down"];

    public FaceDownPick Pick { get; set; }
}
