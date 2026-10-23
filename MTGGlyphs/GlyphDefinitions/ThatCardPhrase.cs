namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Cards named earlier in the line, whatever their type or of one type, optionally counted: "that card", "those cards", "those two cards", "those land cards", "the exiled card", "the exiled cards", "the chosen cards", "the last chosen card", "the other cards". ("the discarded card" is a <see cref="SacrificedPermanent"/>.)</summary>
/// <exampledoc>Faceless Butcher</exampledoc>
/// <examplecapture>the exiled card</examplecapture>
[Dependent]
public class ThatCardPhrase : Glyph
{
    public override Nib[] Nibs => [Alt("that", "those", "the exiled", "the chosen", "the last chosen", "the other"), Prop(Count), Prop(CardType), Plural("card")];

    [Optional]
    public Quantity? Count { get; set; }
    [Optional]
    public CardType? CardType { get; set; }
}
