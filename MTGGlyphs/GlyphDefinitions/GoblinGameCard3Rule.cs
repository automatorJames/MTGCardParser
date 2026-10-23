namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Goblin Game, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Goblin Game</exampledoc>
[TokenizationOrder(-1)]
public class GoblinGameCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player hides at least one item, then all players reveal them simultaneously"];
}
