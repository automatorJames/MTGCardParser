namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Shuffling the cards left over from a reveal or search back into a library: "shuffle the rest into your library", "(that player) shuffles the rest into their library".</summary>
/// <exampledoc>Genesis Hydra</exampledoc>
/// <examplecapture>shuffle the rest into your library</examplecapture>
public class ShuffleRestIntoLibrary : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Verb), "the rest into", Prop(Library)];

    public ShuffleVerb Verb { get; set; }
    public PlayersZone Library { get; set; }
}
