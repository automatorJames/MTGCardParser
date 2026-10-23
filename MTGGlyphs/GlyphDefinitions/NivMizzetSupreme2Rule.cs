namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Niv-Mizzet, Supreme, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Niv-Mizzet, Supreme</exampledoc>
[TokenizationOrder(-1)]
public class NivMizzetSupreme2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each instant and sorcery card in your graveyard that's exactly two colors has jump-start"];
}
