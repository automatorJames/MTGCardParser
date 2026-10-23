namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Midnight Tilling, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Midnight Tilling</exampledoc>
[TokenizationOrder(-1)]
public class MidnightTillingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["mill four cards, then you may return a permanent card from among them to your hand"];
}
