namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Candles of Leng, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Candles of Leng</exampledoc>
[TokenizationOrder(-1)]
public class CandlesOfLengRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it has the same name as a card in your graveyard, put it into your graveyard"];
}
