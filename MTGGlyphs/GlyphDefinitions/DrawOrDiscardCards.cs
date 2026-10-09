namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Drawing or discarding a number of cards: "draw two cards", "discard three cards".</summary>
/// <exampledoc>Bazaar of Baghdad</exampledoc>
/// <examplecapture>draw two cards</examplecapture>
[Dependent]
public class DrawOrDiscardCards : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(CardVerb), Prop(Quantity), Plural("card")];

    public CardVerb CardVerb { get; set; }
    public Quantity Quantity { get; set; }
}

[OptionalPlural]
public enum CardVerb
{
    Draw,
    Discard
}