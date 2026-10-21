namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Exiling cards from the top of a library, often to play them later (impulse draw): "exile the top card of your library", "exile the top two cards of your library", "exile the top three cards of target opponent's library", "… face down". Also a predicate after a subject, "that player exiles the top card of their library" (ingest), and the second of two actions.</summary>
/// <exampledoc>Alania's Pathmaker</exampledoc>
/// <examplecapture>exile the top card of your library</examplecapture>
public class ExileTopOfLibrary : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("exiles?"), Prop(What), Prop(FaceDown)];

    public OneOf<RevealedTopCard, LookedAtTopCard, RevealedTop> What { get; set; }
    [RegexPattern("face down")]
    public bool FaceDown { get; set; }
}
