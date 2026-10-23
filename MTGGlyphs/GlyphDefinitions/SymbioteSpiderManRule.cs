namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Symbiote Spider-Man, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Symbiote Spider-Man</exampledoc>
[TokenizationOrder(-1)]
public class SymbioteSpiderManRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it gains this card's other abilities"];
}
