namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Bubbling Cauldron, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bubbling Cauldron</exampledoc>
[TokenizationOrder(-1)]
public class BubblingCauldronCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{1}, {t}, sacrifice a creature named festering newt: each opponent loses 4 life"];
}
