namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Discarding (or drawing) cards already identified: "that player discards it", "the player discards that card", "then that player discards those cards", "discard it".</summary>
/// <exampledoc>Doomsday Specter</exampledoc>
/// <examplecapture>discards that card</examplecapture>
public class DiscardThatCard : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Verb), Prop(Card)];

    public CardVerb Verb { get; set; }
    public OneOf<It, Them, ThatCardPhrase> Card { get; set; }
}
