namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ertai's Meddling, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ertai's Meddling</exampledoc>
[TokenizationOrder(-1)]
public class ErtaisMeddlingRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["at the beginning of each of that player's upkeeps, if that card is exiled, remove a delay counter from it"];
}
