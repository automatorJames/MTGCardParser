namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Aura Barbs, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aura Barbs</exampledoc>
[TokenizationOrder(-1)]
public class AuraBarbsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each enchantment deals 2 damage to its controller, then each aura attached to a creature deals 2 damage to the creature it's attached to"];
}
