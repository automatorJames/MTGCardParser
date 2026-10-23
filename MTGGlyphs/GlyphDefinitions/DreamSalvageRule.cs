namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dream Salvage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dream Salvage</exampledoc>
[TokenizationOrder(-1)]
public class DreamSalvageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw cards equal to the number of cards target opponent discarded this turn"];
}
