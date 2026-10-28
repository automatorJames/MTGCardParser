namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Struggle for Sanity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Struggle for Sanity</exampledoc>
[TokenizationOrder(-1)]
public class StruggleForSanityRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player returns the cards they exiled this way to their hand and puts the rest into their graveyard"];
}
