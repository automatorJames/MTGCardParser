namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A card being in a zone other than the battlefield, as a condition: "this card is in your graveyard", "{this} is in your hand".</summary>
/// <exampledoc>Bridge from Below</exampledoc>
/// <examplecapture>this card is in your graveyard</examplecapture>
[Dependent]
public class CardIsInZone : Glyph
{
    public override Nib[] Nibs => [Prop(Card), "is in", Prop(Zone)];

    public OneOf<This, ThisCreature, ThatCardPhrase> Card { get; set; }
    public PlayersZone Zone { get; set; }
}
