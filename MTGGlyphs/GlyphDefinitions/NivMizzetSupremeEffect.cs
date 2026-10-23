namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Niv-Mizzet, Supreme, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Niv-Mizzet, Supreme</exampledoc>
[Dependent]
public class NivMizzetSupremeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast that card from your graveyard by discarding a card in addition to paying its other costs"];
}
