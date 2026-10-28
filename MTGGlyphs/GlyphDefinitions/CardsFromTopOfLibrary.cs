namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that many cards from the top of your library", "a number of cards from the top of your library equal to the sacrificed creature's power", "twice x cards from the top of your library": a counted run of cards off the top of a library, the count named before "from the top".</summary>
/// <exampledoc>Feldon, Ronom Excavator</exampledoc>
/// <examplecapture>that many cards from the top of your library</examplecapture>
[Dependent]
public class CardsFromTopOfLibrary : Glyph
{
    public override Nib[] Nibs => [Prop(Count), "cards from the top of", Prop(Whose), Pattern("librar(y|ies)"), Prop(EqualTo)];

    public OneOf<TopCardCount?, Quantity?> Count { get; set; }
    public Owner Whose { get; set; }
    [Optional]
    public EqualTo EqualTo { get; set; }
}
