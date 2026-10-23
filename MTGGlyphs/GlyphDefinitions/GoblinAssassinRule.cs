namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Goblin Assassin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Goblin Assassin</exampledoc>
[TokenizationOrder(-1)]
public class GoblinAssassinRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player whose coin comes up tails sacrifices a creature of their choice"];
}
