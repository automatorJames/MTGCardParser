namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A trigger on cards (not permanents) changing zones: "whenever one or more cards leave your graveyard, …", "whenever a creature card leaves your graveyard, …", "whenever one or more land cards are put into your graveyard from anywhere, …".</summary>
/// <exampledoc>Willow Geist</exampledoc>
/// <examplecapture>whenever one or more cards leave your graveyard, put a +1/+1 counter on this creature</examplecapture>
public class CardZoneTrigger : Glyph
{
    public override Nib[] Nibs => [Alt("when", "whenever"), Prop(Cards), Prop(Event), Prop(During), ",", Prop(Effect)];

    public OneOf<OneOrMoreCards, AnyCard> Cards { get; set; }
    public CardZoneEvent Event { get; set; }
    [Optional]
    public DuringTurn During { get; set; }
    public DynamicGlyph Effect { get; set; }
}
