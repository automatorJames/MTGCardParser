namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Total War, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Total War</exampledoc>
[TokenizationOrder(-1)]
public class TotalWarCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a player attacks with one or more creatures, destroy all untapped non-wall creatures that player controls that didn't attack, except for creatures the player hasn't controlled continuously since the beginning of the turn"];
}
