namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Leyline of Combustion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Leyline of Combustion</exampledoc>
[TokenizationOrder(-1)]
public class LeylineOfCombustionCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you and/or at least one permanent you control becomes the target of a spell or ability an opponent controls, this enchantment deals 2 damage to that player"];
}
