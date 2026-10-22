namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Shuffling a whole hand into its owner's library: "shuffle the cards from your hand into your library", "shuffles the cards from their hand into their library" (wheels, usually followed by "then draw that many cards").</summary>
/// <exampledoc>Whirlpool Drake</exampledoc>
/// <examplecapture>shuffle the cards from your hand into your library</examplecapture>
[Dependent]
public class ShuffleHandIntoLibrary : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Alt("shuffle", "shuffles"), "the cards from", Prop(Whose), "hand into", Prop(LibraryOwner), "library"];

    public Whose Whose { get; set; }
    public Whose LibraryOwner { get; set; }
}
