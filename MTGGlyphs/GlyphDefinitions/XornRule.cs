namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Xorn, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Xorn</exampledoc>
[TokenizationOrder(-1)]
public class XornRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you would create one or more treasure tokens, instead create those tokens plus an additional treasure token"];
}
