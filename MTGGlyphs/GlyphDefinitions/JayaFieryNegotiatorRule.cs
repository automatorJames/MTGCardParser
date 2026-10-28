namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Jaya, Fiery Negotiator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jaya, Fiery Negotiator</exampledoc>
[TokenizationOrder(-1)]
public class JayaFieryNegotiatorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you attack this turn, jaya deals damage equal to the number of attacking creatures to that creature"];
}
