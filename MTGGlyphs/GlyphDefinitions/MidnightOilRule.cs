namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Midnight Oil, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Midnight Oil</exampledoc>
[TokenizationOrder(-1)]
public class MidnightOilRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["your maximum hand size is equal to the number of hour counters on this enchantment"];
}
