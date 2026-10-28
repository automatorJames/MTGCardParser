namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Shadrix Silverquill, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shadrix Silverquill</exampledoc>
[TokenizationOrder(-1)]
public class ShadrixSilverquillRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each mode must target a different player"];
}
