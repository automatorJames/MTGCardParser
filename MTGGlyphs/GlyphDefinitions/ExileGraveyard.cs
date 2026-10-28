namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Exiling whole graveyards: "exile target player's graveyard", "exile all graveyards", "exile each opponent's graveyard".</summary>
/// <exampledoc>Bojuka Bog</exampledoc>
/// <examplecapture>exile target player's graveyard</examplecapture>
public class ExileGraveyard : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["exile", Prop(Whose), Plural("graveyard")];

    public OneOf<TargetPossessive, Whose?> Whose { get; set; }
}
