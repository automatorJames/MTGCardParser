namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Cards in a zone, as a group that's the subject of a rule: "cards in graveyards", "creature cards in your graveyard", "permanent cards in graveyards", "creature cards in graveyards and libraries".</summary>
/// <exampledoc>Ground Seal</exampledoc>
/// <examplecapture>cards in graveyards</examplecapture>
[Dependent]
public class CardsInZoneGroup : Glyph
{
    public override Nib[] Nibs => [Prop(Cards), "in", Prop(Zone)];

    public OneOf<QualifiedCards, CardsNoun?> Cards { get; set; }
    public OneOf<ManyOf<AllPlayersZone>, AllPlayersZone?, PlayersZone> Zone { get; set; }
}
