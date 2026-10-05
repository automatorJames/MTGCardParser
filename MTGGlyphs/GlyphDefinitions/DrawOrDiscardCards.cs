namespace MTGGlyphs.GlyphDefinitions;

public class DrawOrDiscardCards : Glyph
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
