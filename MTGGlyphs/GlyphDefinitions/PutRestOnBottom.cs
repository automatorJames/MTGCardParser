namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"put the rest on the bottom of your library in a random order", "put the rest into your graveyard", "put the rest of the revealed cards on the bottom of your library in any order", "put the rest back on top of your library in any order", "put all cards revealed this way on the bottom of your library": where the cards left over from a look-at/reveal effect go. Also the second of two actions: "… and put the rest into your graveyard", and said of a player: "that player puts all cards revealed this way into their graveyard".</summary>
/// <exampledoc>Impulse</exampledoc>
/// <examplecapture>put the rest on the bottom of your library in any order</examplecapture>
public class PutRestOnBottom : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("puts?"), Prop(Rest), Prop(Back), Prop(Destination)];

    public LeftoverCards Rest { get; set; }
    [Optional]
    [RegexPattern("back")]
    public bool? Back { get; set; }
    public OneOf<LibraryPlacement, IntoZone> Destination { get; set; }
}
