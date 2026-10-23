namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Goblin Game, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Goblin Game</exampledoc>
[TokenizationOrder(-1)]
public class GoblinGameCard4Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player loses life equal to the number of items they revealed"];
}
