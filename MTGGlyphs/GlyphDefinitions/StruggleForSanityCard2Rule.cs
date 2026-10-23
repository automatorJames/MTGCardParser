namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Struggle for Sanity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Struggle for Sanity</exampledoc>
[TokenizationOrder(-1)]
public class StruggleForSanityCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["repeat this process until all cards in that hand have been exiled"];
}
