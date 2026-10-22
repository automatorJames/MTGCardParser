namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"a dragon card from your hand", "this card from your opening hand", "any number of red cards in your hand": a card of a kind, or this card, shown from a hand (an opening hand, for the Chancellors' pregame reveal).</summary>
/// <exampledoc>Draconic Roar</exampledoc>
/// <examplecapture>a dragon card from your hand</examplecapture>
[Dependent]
public class RevealedFromHand : Glyph
{
    public override Nib[] Nibs => [Prop(Wanted), Alt("from", "in"), Prop(Whose), Prop(Opening), "hand"];

    public OneOf<WantedCards, ThisCreature> Wanted { get; set; }
    public Whose Whose { get; set; }
    [Optional]
    [RegexPattern("opening")]
    public bool Opening { get; set; }
}
