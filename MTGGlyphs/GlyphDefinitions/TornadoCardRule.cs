namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tornado, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tornado</exampledoc>
[TokenizationOrder(-1)]
public class TornadoCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{2}{g}, pay 3 life for each velocity counter on this enchantment: destroy target permanent and put a velocity counter on this enchantment"];
}
