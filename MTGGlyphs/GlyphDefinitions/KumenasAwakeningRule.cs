namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kumena's Awakening, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kumena's Awakening</exampledoc>
[TokenizationOrder(-1)]
public class KumenasAwakeningRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you have the city's blessing, instead only you draw a card"];
}
