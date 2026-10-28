namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A card being in a zone other than the battlefield, as a condition: "this card is in your graveyard", "{this} is in your hand", or in exile in some state: "this card is suspended", "this card is exiled with an egg counter on it".</summary>
/// <exampledoc>Bridge from Below</exampledoc>
/// <examplecapture>this card is in your graveyard</examplecapture>
[Dependent]
public class CardIsInZone : Glyph
{
    public override Nib[] Nibs => [Prop(Card), "is", Prop(Place)];

    public OneOf<This, ThisCreature, ThatCardPhrase> Card { get; set; }
    public OneOf<InZone, ExiledState> Place { get; set; }
}
