namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Market Gnome, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Market Gnome</exampledoc>
[TokenizationOrder(-1)]
public class MarketGnomeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when this creature is exiled from the battlefield while you're activating a craft ability, you gain 1 life and draw a card"];
}
