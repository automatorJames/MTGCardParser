namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Searching a library for cards and putting them somewhere (or exiling them), then shuffling: "search your library for a basic land card, reveal it, put it into your hand, then shuffle", "you may search your library for a card named {this}, put it onto the battlefield tapped, then shuffle", "target player searches their library for a basic land card, puts it onto the battlefield tapped, then shuffles".</summary>
/// <exampledoc>Borderland Ranger</exampledoc>
/// <examplecapture>you may search your library for a basic land card, reveal it, put it into your hand, then shuffle</examplecapture>
public class SearchLibrary : Glyph
{
    public override Nib[] Nibs => [Prop(Searcher), Alt("search", "searches"), Alt("your library", "their library"), "for", Prop(Wanted), ",", Prop(Revealed), Prop(Placement), ", then", Alt("shuffle", "shuffles")];

    [Optional]
    public LibrarySearcher Searcher { get; set; }
    public WantedCards Wanted { get; set; }
    [RegexPattern("reveals? (?:it|them|that card|those cards),")]
    public bool Revealed { get; set; }
    public OneOf<PutFoundCards, ExileFoundCards> Placement { get; set; }
}
