namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Worldwalker Helm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Worldwalker Helm</exampledoc>
[TokenizationOrder(-1)]
public class WorldwalkerHelmRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you would create one or more artifact tokens, instead create those tokens plus an additional map token"];
}
