namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rite of Ruin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rite of Ruin</exampledoc>
/// <examplecapture>each player sacrifices one permanent of their choice of the first type, sacrifices two of their choice of the second type, then sacrifices three of their choice of the third type</examplecapture>
[TokenizationOrder(-1)]
public class RiteOfRuinRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player sacrifices one permanent of their choice of the first type, sacrifices two of their choice of the second type, then sacrifices three of their choice of the third type"];
}
