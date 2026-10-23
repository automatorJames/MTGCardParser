namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Incriminate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Incriminate</exampledoc>
[TokenizationOrder(-1)]
public class IncriminateRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player sacrifices one of them of their choice"];
}
