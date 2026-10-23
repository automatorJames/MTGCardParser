namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"return enchanted creature card to the battlefield under your control and attach {this} to it": putting a card back onto the battlefield, possibly naming the zone it comes from ("this card from your graveyard", "a creature card from your graveyard", "all enchantment cards from your graveyard", "the top creature card of your graveyard"), or what a player does, "(each player) returns all artifact cards from their graveyard to the battlefield". Also a predicate, so "you may return target creature card from your graveyard to the battlefield" resolves, and the second of two actions.</summary>
/// <exampledoc>Animate Dead</exampledoc>
/// <examplecapture>return enchanted creature card to the battlefield under your control and attach {this} to it</examplecapture>
public class ReturnToBattlefield : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("returns?"), Prop(Returned), Opt("card"), Prop(Destination), Prop(AndThen)];

    public OneOf<OneOf<PermanentFromZone, AllCardsFromZone, TopCardOfGraveyard>, TargetCardInZone, OneOf<PermanentPhrase, IndefiniteCardInZone>> Returned { get; set; }
    public ToTheBattlefieldUnderControl Destination { get; set; }
    [Optional]
    public AndThen AndThen { get; set; }
}
