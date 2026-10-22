namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A regeneration effect: "regenerate {this}", "regenerate target creature", "regenerate each creature you control", "regenerate all creatures" - sets up a regeneration shield for each permanent. Also a predicate, so "you may regenerate it" resolves, and the second of two actions.</summary>
/// <exampledoc>Drudge Skeletons</exampledoc>
/// <examplecapture>regenerate {this}</examplecapture>
public class Regenerate : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["regenerate", Prop(Regenerated)];

    public OneOf<PermanentPhrase, EachPermanents, AllPermanents> Regenerated { get; set; }
}
