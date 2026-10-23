namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Flash of Defiance, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Flash of Defiance</exampledoc>
[TokenizationOrder(-1)]
public class FlashOfDefianceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["green creatures and white creatures can't block this turn"];
}
