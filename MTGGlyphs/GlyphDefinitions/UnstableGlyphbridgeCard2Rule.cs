namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Unstable Glyphbridge // Sandswirl Wanderglyph, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Unstable Glyphbridge // Sandswirl Wanderglyph</exampledoc>
[TokenizationOrder(-1)]
public class UnstableGlyphbridgeCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then destroy all creatures except creatures chosen this way"];
}
