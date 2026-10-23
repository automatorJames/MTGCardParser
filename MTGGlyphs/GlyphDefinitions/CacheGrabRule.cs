namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cache Grab, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cache Grab</exampledoc>
[TokenizationOrder(-1)]
public class CacheGrabRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you control a squirrel or returned a squirrel card to your hand this way, create a food token"];
}
