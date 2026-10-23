namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gaea's Balance, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gaea's Balance</exampledoc>
[TokenizationOrder(-1)]
public class GaeasBalanceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["search your library for a land card of each basic land type, put those cards onto the battlefield, then shuffle"];
}
