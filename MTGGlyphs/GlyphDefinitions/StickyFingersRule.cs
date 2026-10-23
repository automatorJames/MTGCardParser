namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sticky Fingers, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sticky Fingers</exampledoc>
[TokenizationOrder(-1)]
public class StickyFingersRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the token is an artifact with \"{t}, sacrifice this token: add one mana of any color"];
}
