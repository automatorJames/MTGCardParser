namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An object of a kind having been put into a zone this turn, as a condition: "an artifact or creature was put into a graveyard from the battlefield this turn", "a permanent was put into your hand from the battlefield this turn", "a creature card was put into your graveyard from anywhere this turn".</summary>
/// <exampledoc>Ichor Shade</exampledoc>
/// <examplecapture>an artifact or creature was put into a graveyard from the battlefield this turn</examplecapture>
[Dependent]
public class ObjectPutIntoZoneThisTurn : Glyph
{
    public override Nib[] Nibs => [Prop(Object), "was put into", Opt("a"), Prop(Zone), Prop(Origin), Prop(Turn)];

    public OneOf<IndefinitePermanent, IndefiniteCard> Object { get; set; }
    public OneOf<PlayersZone, NonBattlefieldZone?> Zone { get; set; }
    public ArrivalOrigin Origin { get; set; }
    public TurnSpan Turn { get; set; }
}
