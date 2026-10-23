namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of False Orders, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>False Orders</exampledoc>
[TokenizationOrder(-1)]
public class FalseOrders2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may have it block an attacking creature of your choice"];
}
