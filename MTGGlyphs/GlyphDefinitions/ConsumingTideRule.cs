namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Consuming Tide, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Consuming Tide</exampledoc>
[TokenizationOrder(-1)]
public class ConsumingTideRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return all nonland permanents not chosen this way to their owners' hands"];
}
