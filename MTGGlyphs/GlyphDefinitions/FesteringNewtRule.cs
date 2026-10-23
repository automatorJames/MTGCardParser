namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Festering Newt, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Festering Newt</exampledoc>
[TokenizationOrder(-1)]
public class FesteringNewtRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that creature gets -4/-4 instead if you control a creature named bogbrew witch"];
}
