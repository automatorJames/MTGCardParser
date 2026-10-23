namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A count, as an amount: "the number of swamps you control", "the number of other attacking creatures", "the number of doom counters on it", "the number of cards in your hand", "the number of basic land types among lands you control", "the number of creatures in your party", "the number of cards revealed this way", "the number of cards exiled with it", "the number of +1/+1 counters removed this way", "the number of spells you've cast this turn", "the number of differently named lands you control", "the total number of instant and sorcery cards you own in exile and in your graveyard". The party is tried before plain permanents, which would stop at "creatures"; spells and cards handled this turn before cards in a zone. "other" excludes the source itself; "differently named" counts distinct names.</summary>
/// <exampledoc>Nightmare</exampledoc>
/// <examplecapture>the number of swamps you control</examplecapture>
[Dependent]
public class NumberOf : Glyph
{
    public override Nib[] Nibs => ["the", Opt("total"), "number of", Prop(Other), Prop(DifferentlyNamed), Prop(Counted)];

    [Optional]
    [RegexPattern("other")]
    public bool? Other { get; set; }
    [Optional]
    [RegexPattern("differently named")]
    public bool? DifferentlyNamed { get; set; }
    public OneOf<OneOf<PartyCreatures, Permanents>, OneOf<CardsHandledThisTurn, CountersOn, CountersRemovedThisWay>, OneOf<OneOf<AffectedThisWay, CardsExiledWithThis, OwnedCardsInZones>, CardsInZone, DistinctKinds>> Counted { get; set; }
}
