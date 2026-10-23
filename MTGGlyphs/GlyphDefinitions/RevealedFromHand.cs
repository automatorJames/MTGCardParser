namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"a dragon card from your hand", "this card from your opening hand", "any number of red cards in your hand", "a card at random from their hand", "a number of cards from their hand equal to the number of swamps you control": a card of a kind, or this card, shown from a hand (an opening hand, for the Chancellors' pregame reveal), possibly chosen at random, or a number of them set by an "equal to …".</summary>
/// <exampledoc>Draconic Roar</exampledoc>
/// <examplecapture>a dragon card from your hand</examplecapture>
[Dependent]
public class RevealedFromHand : Glyph
{
    public override Nib[] Nibs => [Prop(Wanted), Prop(AtRandom), Alt("from", "in"), Prop(Whose), Prop(Opening), "hand", Prop(Count)];

    public OneOf<WantedCards, ThisCreature> Wanted { get; set; }
    [Optional]
    [RegexPattern("at random")]
    public bool AtRandom { get; set; }
    public Whose Whose { get; set; }
    [Optional]
    [RegexPattern("opening")]
    public bool Opening { get; set; }
    [Optional]
    public EqualTo Count { get; set; }
}
