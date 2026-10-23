namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Quicken, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Quicken</exampledoc>
[TokenizationOrder(-1)]
public class QuickenRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the next sorcery spell you cast this turn can be cast as though it had flash"];
}
