namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Cards having left a zone this turn, as a condition: "a card left your graveyard this turn", "three or more cards left your graveyard this turn", "one or more cards left your graveyard this turn".</summary>
/// <exampledoc>Primary Research</exampledoc>
/// <examplecapture>a card left your graveyard this turn</examplecapture>
[Dependent]
public class CardsLeftZoneThisTurn : Glyph
{
    public override Nib[] Nibs => [Prop(Count), Plural("card"), "left", Prop(Zone), "this turn"];

    public OneOf<AtLeastCount, Quantity?> Count { get; set; }
    public PlayersZone Zone { get; set; }
}
