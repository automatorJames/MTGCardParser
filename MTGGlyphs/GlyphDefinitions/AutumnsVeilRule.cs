namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Autumn's Veil, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Autumn's Veil</exampledoc>
/// <examplecapture>spells you control can't be countered by blue or black spells this turn, and creatures you control can't be the targets of blue or black spells this turn</examplecapture>
[TokenizationOrder(-1)]
public class AutumnsVeilRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spells you control can't be countered by blue or black spells this turn, and creatures you control can't be the targets of blue or black spells this turn"];
}
