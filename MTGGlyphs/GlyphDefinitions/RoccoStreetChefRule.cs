namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rocco, Street Chef, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rocco, Street Chef</exampledoc>
[TokenizationOrder(-1)]
public class RoccoStreetChefRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until your next end step, each player may play the card they exiled this way"];
}
