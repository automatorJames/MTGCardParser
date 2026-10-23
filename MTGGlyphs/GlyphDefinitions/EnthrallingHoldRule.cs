namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Enthralling Hold, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Enthralling Hold</exampledoc>
[TokenizationOrder(-1)]
public class EnthrallingHoldRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can't choose an untapped creature as this spell's target as you cast it"];
}
