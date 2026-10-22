namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// Sacrificing a permanent: "sacrifice {this}", "sacrifice a creature", "sacrifice two islands", "sacrifice them", "sacrifices the rest", or, as what a subject does, "(that creature's
/// controller) sacrifices it", "(each player) sacrifices x creatures of their choice", or as a gerund cost: "by sacrificing a creature". A single permanent's chooser is part of <see cref="IndefinitePermanent"/>.
/// </summary>
/// <exampledoc>Animate Dead</exampledoc>
/// <examplecapture>sacrifices it</examplecapture>
public class Sacrifice : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("sacrific(?:es?|ing)"), Prop(Sacrificed), Prop(ChosenBy)];

    public OneOf<PermanentPhrase, IndefinitePermanent, OneOf<CountedPermanents, Them, TheRest>> Sacrificed { get; set; }
    [Optional]
    public OfPlayersChoice ChosenBy { get; set; }
}
