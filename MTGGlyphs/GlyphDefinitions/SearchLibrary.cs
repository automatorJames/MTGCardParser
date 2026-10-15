namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Searching your library for cards and putting them somewhere, then shuffling: "search your library for a basic land card, reveal it, put it into your hand, then shuffle", "you may search your library for a card named {this}, put it onto the battlefield tapped, then shuffle", "… reveal it, then shuffle and put that card on top".</summary>
/// <exampledoc>Borderland Ranger</exampledoc>
/// <examplecapture>you may search your library for a basic land card, reveal it, put it into your hand, then shuffle</examplecapture>
public class SearchLibrary : Glyph
{
    public override Nib[] Nibs => [Prop(May), "search your library for", Prop(Wanted), ",", Prop(Revealed), Alt("put it", "put that card", "put them", "put those cards"), Alt("into your hand", "into your graveyard", "onto the battlefield"), Prop(Tapped), ", then shuffle"];

    [RegexPattern("you may")]
    public bool May { get; set; }
    public WantedCards Wanted { get; set; }
    [RegexPattern("reveal (?:it|them|that card|those cards),")]
    public bool Revealed { get; set; }
    [RegexPattern("tapped")]
    public bool Tapped { get; set; }
}
