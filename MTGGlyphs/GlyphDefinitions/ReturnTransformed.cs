namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Returning a card that was exiled as a cost back to the battlefield with its other face up: "return this card transformed under its owner's control" (craft).</summary>
/// <exampledoc>Tithing Blade // Consuming Sepulcher</exampledoc>
/// <examplecapture>return this card transformed under its owner's control</examplecapture>
public class ReturnTransformed : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("returns?"), Prop(Card), "transformed under", Prop(Whose), "control"];

    public PermanentPhrase Card { get; set; }
    public Whose Whose { get; set; }
}
