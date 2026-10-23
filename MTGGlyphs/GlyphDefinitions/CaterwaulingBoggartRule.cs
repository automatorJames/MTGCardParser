namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Caterwauling Boggart, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Caterwauling Boggart</exampledoc>
[TokenizationOrder(-1)]
public class CaterwaulingBoggartRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["goblins you control and elementals you control have menace"];
}
