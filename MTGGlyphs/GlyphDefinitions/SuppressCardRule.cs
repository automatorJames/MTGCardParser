namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Suppress, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Suppress</exampledoc>
[TokenizationOrder(-1)]
public class SuppressCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["at the beginning of the end step of that player's next turn, that player returns those cards to their hand"];
}
