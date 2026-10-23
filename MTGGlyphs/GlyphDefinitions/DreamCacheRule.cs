namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dream Cache, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dream Cache</exampledoc>
[TokenizationOrder(-1)]
public class DreamCacheRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw three cards, then put two cards from your hand both on top of your library or both on the bottom of your library"];
}
