namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A condition on a revealed card's name matching a name chosen earlier: "that card has the chosen name" (Cursed Scroll, Petra Sphinx).</summary>
/// <exampledoc>Cursed Scroll</exampledoc>
/// <examplecapture>that card has the chosen name</examplecapture>
[Dependent]
public class CardHasChosenName : Glyph
{
    public override Nib[] Nibs => [Prop(Card), "has the chosen name"];

    public ThatCardPhrase Card { get; set; }
}
