namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A regeneration effect: "regenerate {this}", "regenerate target creature" - sets up a regeneration shield for one permanent. Also a predicate, so "you may regenerate it" resolves, and the second of two actions.</summary>
/// <exampledoc>Drudge Skeletons</exampledoc>
/// <examplecapture>regenerate {this}</examplecapture>
public class Regenerate : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["regenerate", Prop(Regenerated)];

    public PermanentPhrase Regenerated { get; set; }
}
