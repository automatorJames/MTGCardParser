namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dream Leash, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dream Leash</exampledoc>
[TokenizationOrder(-1)]
public class DreamLeashRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can't choose an untapped permanent as this spell's target as you cast it"];
}
