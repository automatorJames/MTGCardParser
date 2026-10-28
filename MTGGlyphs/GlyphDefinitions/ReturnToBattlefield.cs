namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"return enchanted creature card to the battlefield under your control and attach {this} to it": putting a card back onto the battlefield, possibly naming the zone it comes from ("this card from your graveyard", "a creature card from your graveyard", "all enchantment cards from your graveyard", "the top creature card of your graveyard", "up to two creature cards of that type from your graveyard", "two creature cards at random from your graveyard"), or what a player does, "(each player) returns all artifact cards from their graveyard to the battlefield", or cards named earlier ("them", "each of them", "each card put into a graveyard this way"). Also a predicate, so "you may return target creature card from your graveyard to the battlefield" resolves, and the second of two actions. A counted run of cards is tried after a single indefinite one.</summary>
/// <exampledoc>Animate Dead</exampledoc>
/// <examplecapture>return enchanted creature card to the battlefield under your control and attach {this} to it</examplecapture>
public class ReturnToBattlefield : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("returns?"), Prop(Returned), Opt("card"), Prop(Destination), Prop(AndThen)];

    public OneOf<PermanentFromZone, AllCardsFromZone, TopCardOfGraveyard, TargetCardInZone, PermanentPhrase, IndefiniteCardInZone, Them, EachOf, ObjectsAffectedThisWay, CountedCardsInZone> Returned { get; set; }
    public ToTheBattlefieldUnderControl Destination { get; set; }
    [Optional]
    public AndThen AndThen { get; set; }
}
