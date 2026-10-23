namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Enduring Courage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Enduring Courage</exampledoc>
[TokenizationOrder(-1)]
public class EnduringCourage2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it's an enchantment"];
}
