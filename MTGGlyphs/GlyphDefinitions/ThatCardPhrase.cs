namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Cards named earlier in the line, whatever their type or of one type: "that card", "those cards", "those land cards", "the exiled card", "the exiled cards", "the chosen cards". ("the discarded card" is a <see cref="SacrificedPermanent"/>.)</summary>
/// <exampledoc>Faceless Butcher</exampledoc>
/// <examplecapture>the exiled card</examplecapture>
[Dependent]
public class ThatCardPhrase : Glyph
{
    public override Nib[] Nibs => [Alt("that", "those", "the exiled", "the chosen"), Prop(CardType), Plural("card")];

    [Optional]
    public CardType? CardType { get; set; }
}
