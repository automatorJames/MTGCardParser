namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tapestry Warden, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tapestry Warden</exampledoc>
[TokenizationOrder(-1)]
public class TapestryWardenRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each creature you control with toughness greater than its power stations permanents using its toughness rather than its power"];
}
