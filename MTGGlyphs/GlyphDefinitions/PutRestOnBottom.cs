namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"put the rest on the bottom of your library in a random order", "put the rest into your graveyard": where the cards left over from a look-at/reveal effect go. Also the second of two actions: "… and put the rest into your graveyard".</summary>
/// <exampledoc>Impulse</exampledoc>
/// <examplecapture>put the rest on the bottom of your library in any order</examplecapture>
public class PutRestOnBottom : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["put the rest", Prop(Destination)];

    public OneOf<LibraryPlacement, IntoZone> Destination { get; set; }
}
