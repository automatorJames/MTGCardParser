namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sen Triplets, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sen Triplets</exampledoc>
[TokenizationOrder(-1)]
public class SenTripletsRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may play lands and cast spells from that player's hand this turn"];
}
