namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Search that finds a card and leaves it on top of the library: "search your library for a goblin card, reveal it, then shuffle and put that card on top".</summary>
/// <exampledoc>Boggart Harbinger</exampledoc>
/// <examplecapture>search your library for a goblin card, reveal it, then shuffle and put that card on top</examplecapture>
public class SearchLibraryToTop : Glyph
{
    public override Nib[] Nibs => [Prop(May), "search your library for", Prop(Wanted), ",", Prop(Revealed), "then shuffle and put that card on top"];

    [RegexPattern("you may")]
    public bool May { get; set; }
    public WantedCards Wanted { get; set; }
    [RegexPattern("reveal (?:it|them|that card|those cards),")]
    public bool Revealed { get; set; }
}
