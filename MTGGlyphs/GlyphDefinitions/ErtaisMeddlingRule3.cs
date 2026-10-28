namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ertai's Meddling, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ertai's Meddling</exampledoc>
[TokenizationOrder(-1)]
public class ErtaisMeddlingRule3 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the card has no delay counters on it, the player puts it onto the stack as a copy of the original spell"];
}
