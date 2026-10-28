namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sanctum of Shattered Heights, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sanctum of Shattered Heights</exampledoc>
[TokenizationOrder(-1)]
public class SanctumOfShatteredHeightsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{1}, discard a land card or shrine card:", Nib.This, "deals x damage to target creature or planeswalker, where x is the number of shrines you control"];
}
