namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the top four cards of your library", "the top three cards of target player's library": several cards from the top of a library, as what's looked at, revealed, exiled or put somewhere.</summary>
/// <exampledoc>Index</exampledoc>
/// <examplecapture>the top five cards of your library</examplecapture>
[Dependent]
public class TopCardsOfLibrary : Glyph
{
    public override Nib[] Nibs => ["the top", Prop(Quantity), Plural("card"), "of", Prop(Owner), "library"];

    public Quantity Quantity { get; set; }
    public Owner Owner { get; set; }
}
