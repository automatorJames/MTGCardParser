namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Neverending Torment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Neverending Torment</exampledoc>
[TokenizationOrder(-1)]
public class NeverendingTormentRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["search target player's library for x cards, where x is the number of cards in your hand, and exile them"];
}
