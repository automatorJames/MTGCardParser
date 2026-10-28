namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// Sacrificing a permanent: "sacrifice {this}", "sacrifice a creature", "sacrifice two islands", "sacrifice them", "sacrifices the rest", a legend by its short name ("sacrifice kagemaro"), a list of single permanents ("sacrifice a green creature, a white creature, and a blue creature"), or, as what a subject does, "(that creature's
/// controller) sacrifices it", "(each player) sacrifices x creatures of their choice", or as a gerund cost: "by sacrificing a creature". A single permanent's chooser is part of <see cref="IndefinitePermanent"/>.
/// </summary>
/// <exampledoc>Animate Dead</exampledoc>
/// <examplecapture>sacrifices it</examplecapture>
public class Sacrifice : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("sacrific(?:es?|ing)"), Prop(Sacrificed), Prop(ChosenBy)];

    public OneOf<PermanentPhrase, ManyOf<IndefinitePermanent>, IndefinitePermanent, CountedPermanents, Them, TheRest, ShortName> Sacrificed { get; set; }
    [Optional]
    public OfPlayersChoice ChosenBy { get; set; }
}
