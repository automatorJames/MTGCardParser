namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Neriv, Heart of the Storm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Neriv, Heart of the Storm</exampledoc>
[TokenizationOrder(-1)]
public class NerivHeartOfTheStormRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a creature you control that entered this turn would deal damage, it deals twice that much damage instead"];
}
