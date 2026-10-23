namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Returning objects to a zone with the destination first, as Oracle words it when the returned objects are a long phrase: "return to your hand all creature cards in your graveyard that were put there from the battlefield this turn", "return to their owners' hands all creatures with toughness less than or equal to the number of islands you control", "return to your hand target artifact card in your graveyard with lesser mana value", "(each player) returns to the battlefield all artifact, creature, enchantment, and land cards in their graveyard that were put there from the battlefield this turn". The inverted sibling of ReturnCard and ReturnToBattlefield.</summary>
/// <exampledoc>Twilight Shepherd</exampledoc>
/// <examplecapture>return to your hand all cards in your graveyard that were put there from the battlefield this turn</examplecapture>
public class ReturnToZoneFirst : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("returns?"), "to", Prop(Destination), Prop(Returned)];

    public OneOf<PlayersZone, BattlefieldZone?> Destination { get; set; }
    public ReturnedObject Returned { get; set; }
}
