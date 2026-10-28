namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Teferi's Response, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Teferi's Response</exampledoc>
[TokenizationOrder(-1)]
public class TeferisResponseRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a permanent's ability is countered this way, destroy that permanent"];
}
