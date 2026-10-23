namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A count of cards in one zone, said of the zone: "a graveyard has twenty or more cards in it", "a library has twenty or fewer cards in it", "your library has ten or more cards in it".</summary>
/// <exampledoc>Visions of Beyond</exampledoc>
/// <examplecapture>a graveyard has twenty or more cards in it</examplecapture>
[Dependent]
public class ZoneHoldsCards : Glyph
{
    public override Nib[] Nibs => [Opt("a"), Prop(Zone), "has", Prop(Count), "or", Prop(Bound), "cards in it"];

    public OneOf<PlayersZone, NonBattlefieldZone?> Zone { get; set; }
    public Quantity Count { get; set; }
    public CountBound Bound { get; set; }
}
