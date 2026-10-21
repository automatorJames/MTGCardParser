namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An unspecified number of cards: "a card", "two cards" - the object of a keyword action's reminder.</summary>
/// <exampledoc>Ethereal Ambush</exampledoc>
/// <examplecapture>a card</examplecapture>
[Dependent]
public class SomeCards : Glyph
{
    public override Nib[] Nibs => [Prop(Quantity), Plural("card")];

    public Quantity Quantity { get; set; }
}
