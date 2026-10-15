namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card verb applied to cards chosen earlier: "discards those cards".</summary>
/// <exampledoc>Abandon Hope</exampledoc>
/// <examplecapture>discards those cards</examplecapture>
[Dependent]
public class CardVerbThoseCards : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(CardVerb), Prop(Cards), "cards"];

    public CardVerb CardVerb { get; set; }
    public Those Cards { get; set; }
}
