namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gilded Drake, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gilded Drake</exampledoc>
[TokenizationOrder(-1)]
public class GildedDrakeRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this ability still resolves if its target becomes illegal"];
}
