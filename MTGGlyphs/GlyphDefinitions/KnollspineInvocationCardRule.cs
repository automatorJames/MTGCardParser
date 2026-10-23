namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Knollspine Invocation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Knollspine Invocation</exampledoc>
[TokenizationOrder(-1)]
public class KnollspineInvocationCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{x}, discard a card with mana value x: this enchantment deals x damage to any target"];
}
