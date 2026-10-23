namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dark Deal, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dark Deal</exampledoc>
[TokenizationOrder(-1)]
public class DarkDealRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player discards all the cards in their hand, then draws that many cards minus one"];
}
