namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Silver Wyvern, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Silver Wyvern</exampledoc>
[TokenizationOrder(-1)]
public class SilverWyvernRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the new target must be a creature"];
}
