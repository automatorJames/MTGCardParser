namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Raging River, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Raging River</exampledoc>
[TokenizationOrder(-1)]
public class RagingRiverRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then, for each attacking creature you control, choose \"left\" or \"right"];
}
