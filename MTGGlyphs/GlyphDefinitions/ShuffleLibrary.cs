namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Shuffling a library on its own: "shuffle your library", "then shuffle" (after a search), "target player shuffles their library".</summary>
/// <exampledoc>Fork in the Road</exampledoc>
/// <examplecapture>shuffle</examplecapture>
public class ShuffleLibrary : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Verb), Prop(Library)];

    public ShuffleVerb Verb { get; set; }
    [Optional]
    public PlayersZone Library { get; set; }
}
