namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ion Storm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ion Storm</exampledoc>
[TokenizationOrder(-1)]
public class IonStormRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{1}{r}, remove a +1/+1 counter or a charge counter from a permanent you control: this enchantment deals 2 damage to any target"];
}
