namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Goblin Game, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Goblin Game</exampledoc>
[TokenizationOrder(-1)]
public class GoblinGameRule3 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the player who revealed the fewest items then loses half their life, rounded up"];
}
