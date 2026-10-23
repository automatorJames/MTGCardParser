namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Patriarch's Bidding, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Patriarch's Bidding</exampledoc>
[TokenizationOrder(-1)]
public class PatriarchsBiddingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player returns all creature cards of a type chosen this way from their graveyard to the battlefield"];
}
