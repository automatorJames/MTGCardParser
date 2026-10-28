namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A choice made as part of an effect: "choose an opponent", "choose a color and an opponent", "choose a nonland card name", "choose an artifact card name", "an opponent chooses a creature type", "choose a creature type other than wall", "choose another creature you control", "each player chooses three permanents they control", "choose target creature you control and target creature you don't control", "choose target creature card in your graveyard", "choose up to two target creature cards in your graveyard", "chooses an artifact, a creature, an enchantment, and a planeswalker from among the nonland permanents they control", "chooses target player who controls more creatures than they do and is their opponent". Also a predicate, so "you may choose a color" resolves.</summary>
/// <exampledoc>Jihad</exampledoc>
/// <examplecapture>choose a color and an opponent</examplecapture>
public class Choose : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Alt("choose", "chooses"), Prop(Choices), Prop(Except)];

    public OneOf<ManyOf<Choice>, Choice?, TargetCardInZone, CountedTargets, TargetPlayerWhoOutnumbers, PermanentPhrase, IndefinitePermanent, QualifiedCardName, CountedPermanents, ManyOf<SpecificTarget>, KindsFromAmong> Choices { get; set; }
    [Optional]
    public ExcludedKind Except { get; set; }
}
