namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Truth or Tale, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Truth or Tale</exampledoc>
/// <examplecapture>put a card from the chosen pile into your hand, then put all other cards revealed this way on the bottom of your library in any order</examplecapture>
[TokenizationOrder(-1)]
public class TruthOrTaleRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a card from the chosen pile into your hand, then put all other cards revealed this way on the bottom of your library in any order"];
}
