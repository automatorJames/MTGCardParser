namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tannuk, Steadfast Second, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tannuk, Steadfast Second</exampledoc>
[TokenizationOrder(-1)]
public class TannukSteadfastSecondRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["artifact cards and red creature cards in your hand have warp {2}{r}"];
}
