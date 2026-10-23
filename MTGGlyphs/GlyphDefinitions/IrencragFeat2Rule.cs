namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Irencrag Feat, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Irencrag Feat</exampledoc>
[TokenizationOrder(-1)]
public class IrencragFeat2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["add seven {r}"];
}
