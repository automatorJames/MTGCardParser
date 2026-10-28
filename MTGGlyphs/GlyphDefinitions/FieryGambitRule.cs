namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fiery Gambit, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fiery Gambit</exampledoc>
[TokenizationOrder(-1)]
public class FieryGambitRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["flip a coin until you lose a flip or choose to stop flipping"];
}
