namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Minamo's Meddling, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Minamo's Meddling</exampledoc>
[TokenizationOrder(-1)]
public class MinamosMeddlingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that spell's controller reveals their hand, then discards each card with the same name as a card spliced onto that spell"];
}
