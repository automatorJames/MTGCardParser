namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Invasion of Pyrulea // Gargantuan Slabhorn, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Invasion of Pyrulea // Gargantuan Slabhorn</exampledoc>
[TokenizationOrder(-1)]
public class InvasionOfPyruleaRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's a land or double-faced card, draw a card"];
}
