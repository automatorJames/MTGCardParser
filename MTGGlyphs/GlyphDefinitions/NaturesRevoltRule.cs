namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Nature's Revolt, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nature's Revolt</exampledoc>
[TokenizationOrder(-1)]
public class NaturesRevoltRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["all lands are 2/2 creatures that are still lands"];
}
