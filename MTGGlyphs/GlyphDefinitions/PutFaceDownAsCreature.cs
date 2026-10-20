namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Manifesting, as its reminder defines it: "put it onto the battlefield face down as a 2/2 creature" - a predicate.</summary>
/// <exampledoc>Cloudform</exampledoc>
/// <examplecapture>put it onto the battlefield face down as a 2/2 creature</examplecapture>
[Dependent]
public class PutFaceDownAsCreature : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["put", Prop(Moved), "onto the battlefield face down as a", Prop(Stats), "creature"];

    public OneOf<It, Them, PermanentPhrase> Moved { get; set; }
    public PowerToughness Stats { get; set; }
}
