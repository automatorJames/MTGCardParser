namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Balance, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Balance</exampledoc>
[TokenizationOrder(-1)]
public class BalanceCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["players discard cards and sacrifice creatures the same way"];
}
