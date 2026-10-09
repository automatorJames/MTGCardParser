namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// Sacrificing a permanent: "sacrifice {this}", "sacrifice a creature", or, as what a subject does, "(that creature's
/// controller) sacrifices it".
/// </summary>
/// <exampledoc>Animate Dead</exampledoc>
/// <examplecapture>sacrifices it</examplecapture>
public class Sacrifice : Glyph, IPredicate
{
    public override Nib[] Nibs => [Pattern("sacrifices?"), Prop(Sacrificed)];

    public OneOf<PermanentPhrase, IndefinitePermanent> Sacrificed { get; set; }
}
