namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// Sacrificing a permanent: "sacrifice {this}", "sacrifice a creature", "sacrifice two islands", "sacrifice them", or, as what a subject does, "(that creature's
/// controller) sacrifices it", "(each player) sacrifices a creature of their choice", or as a gerund cost: "by sacrificing a creature".
/// </summary>
/// <exampledoc>Animate Dead</exampledoc>
/// <examplecapture>sacrifices it</examplecapture>
public class Sacrifice : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("sacrific(?:es?|ing)"), Prop(Sacrificed), Prop(OfTheirChoice)];

    public OneOf<PermanentPhrase, IndefinitePermanent, OneOf<CountedPermanents, Them>> Sacrificed { get; set; }
    [RegexPattern("of their choice")]
    public bool OfTheirChoice { get; set; }
}
