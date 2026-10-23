namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Coalition Victory, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Coalition Victory</exampledoc>
[TokenizationOrder(-1)]
public class CoalitionVictoryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you win the game if you control a land of each basic land type and a creature of each color"];
}
