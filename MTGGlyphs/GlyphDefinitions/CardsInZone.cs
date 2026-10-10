namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Cards in a player's zone, to count: "cards in their hand".</summary>
/// <exampledoc>Black Vise</exampledoc>
/// <examplecapture>cards in their hand</examplecapture>
[Dependent]
public class CardsInZone : Glyph
{
    public override Nib[] Nibs => ["cards in", Prop(Zone)];

    public PlayersZone Zone { get; set; }
}
