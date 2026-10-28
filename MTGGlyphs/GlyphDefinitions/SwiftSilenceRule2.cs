namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Swift Silence, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Swift Silence</exampledoc>
[TokenizationOrder(-1)]
public class SwiftSilenceRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card for each spell countered this way"];
}
