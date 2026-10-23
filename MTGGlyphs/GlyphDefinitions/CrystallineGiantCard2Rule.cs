namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Crystalline Giant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Crystalline Giant</exampledoc>
[TokenizationOrder(-1)]
public class CrystallineGiantCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a counter of that kind on this creature"];
}
