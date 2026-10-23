namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Risky Move, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Risky Move</exampledoc>
[TokenizationOrder(-1)]
public class RiskyMoveCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when you gain control of this enchantment from another player, choose a creature you control and an opponent"];
}
