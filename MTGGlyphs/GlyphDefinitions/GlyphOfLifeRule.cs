namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Glyph of Life, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Glyph of Life</exampledoc>
[TokenizationOrder(-1)]
public class GlyphOfLifeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever that creature is dealt damage by an attacking creature this turn, you gain that much life"];
}
