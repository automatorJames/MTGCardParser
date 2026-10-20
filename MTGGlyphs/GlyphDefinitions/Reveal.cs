namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Revealing cards: "target opponent reveals their hand", "reveal the top four cards of your library", "reveal a dragon card from your hand". Also a predicate, so "reveal the top card of your library and put that card into your hand" pairs it with the next action, and the second of two actions.</summary>
/// <exampledoc>Distress</exampledoc>
/// <examplecapture>target player reveals their hand</examplecapture>
public class Reveal : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Who), Pattern("reveals?"), Prop(What)];

    [Optional]
    public Subject Who { get; set; }
    public RevealedThing What { get; set; }
}
