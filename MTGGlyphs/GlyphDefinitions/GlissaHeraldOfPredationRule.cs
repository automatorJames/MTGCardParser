namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Glissa, Herald of Predation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Glissa, Herald of Predation</exampledoc>
[TokenizationOrder(-1)]
public class GlissaHeraldOfPredationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• transform all incubator tokens you control"];
}
