namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Biblioplex, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Biblioplex</exampledoc>
[TokenizationOrder(-1)]
public class TheBiblioplexRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["activate only if you have exactly zero or seven cards in hand"];
}
