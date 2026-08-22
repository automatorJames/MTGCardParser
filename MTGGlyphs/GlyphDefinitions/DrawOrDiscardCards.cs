namespace MTGGlyphs.GlyphDefinitions;

public class DrawOrDiscardCards : Glyph
{
    public override Nib[] Nibs => [Prop(CardVerb), Prop(Quantity), "cards?"];

    public CardVerb CardVerb { get; set; }
    public Quantity Quantity { get; set; }
}

[OptionalPlural]
public enum CardVerb
{
    Draw,

    [RegexPattern("discard", "discard angrily")]
    Discard
}