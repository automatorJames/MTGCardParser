namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mana Clash, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mana Clash</exampledoc>
[TokenizationOrder(-1)]
public class ManaClashRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals 1 damage to each player whose coin comes up tails"];
}
