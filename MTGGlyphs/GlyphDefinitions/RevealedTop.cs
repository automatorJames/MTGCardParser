namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the top four cards of your library", "the top three cards of target player's library": several cards from the top of a library.</summary>
/// <exampledoc>Surging Aether</exampledoc>
/// <examplecapture>the top four cards of your library</examplecapture>
[Dependent]
public class RevealedTop : Glyph
{
    public override Nib[] Nibs => ["the top", Prop(Quantity), Plural("card"), "of", Prop(Whose), "library"];

    public Quantity Quantity { get; set; }
    public OneOf<Whose?, PlayerPossessive> Whose { get; set; }
}
