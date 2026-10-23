namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Goblin Game, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Goblin Game</exampledoc>
[TokenizationOrder(-1)]
public class GoblinGameRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if two or more players are tied for fewest, each loses half their life, rounded up"];
}
