namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Silverquill, the Disputant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Silverquill, the Disputant</exampledoc>
[TokenizationOrder(-1)]
public class SilverquillTheDisputantRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each instant and sorcery spell you cast has casualty 1"];
}
