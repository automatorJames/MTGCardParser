namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Discarding a whole hand: "discard your hand", "discards their hand", "discard all the cards in your hand".</summary>
/// <exampledoc>Avaricious Dragon</exampledoc>
/// <examplecapture>discard your hand</examplecapture>
[Dependent]
public class DiscardHand : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Verb), Opt("all the cards in"), Prop(Whose), "hand"];

    public CardVerb Verb { get; set; }
    public Whose Whose { get; set; }
}
