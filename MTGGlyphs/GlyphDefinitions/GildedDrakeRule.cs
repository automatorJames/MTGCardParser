namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gilded Drake, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gilded Drake</exampledoc>
[TokenizationOrder(-1)]
public class GildedDrakeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you don't or can't make an exchange, sacrifice this creature"];
}
