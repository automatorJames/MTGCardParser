namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Corruption of Towashi, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Corruption of Towashi</exampledoc>
[TokenizationOrder(-1)]
public class CorruptionOfTowashiRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a permanent you control transforms or a permanent you control enters transformed, you may draw a card"];
}
