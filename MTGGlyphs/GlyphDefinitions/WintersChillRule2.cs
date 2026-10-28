namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Winter's Chill, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Winter's Chill</exampledoc>
[TokenizationOrder(-1)]
public class WintersChillRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that player pays only {1}, prevent all combat damage that would be dealt to and dealt by that creature this combat"];
}
