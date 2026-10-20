namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Putting some of the cards just looked at or revealed in one place and the rest in another: "put one of them into your hand and the rest on the bottom of your library in any order".</summary>
/// <exampledoc>Impulse</exampledoc>
/// <examplecapture>put one of them into your hand and the rest on the bottom of your library in any order</examplecapture>
public class PutSplitCards : Glyph, IPredicate
{
    public override Nib[] Nibs => ["put", Prop(Split)];

    public SplitLookedAtCards Split { get; set; }
}
