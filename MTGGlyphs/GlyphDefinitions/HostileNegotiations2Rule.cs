namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hostile Negotiations, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hostile Negotiations</exampledoc>
[TokenizationOrder(-1)]
public class HostileNegotiations2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["look at the cards in each pile, then turn a pile of your choice face up"];
}
