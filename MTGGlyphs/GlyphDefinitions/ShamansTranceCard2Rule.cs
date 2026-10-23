namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Shaman's Trance, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shaman's Trance</exampledoc>
[TokenizationOrder(-1)]
public class ShamansTranceCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["other players can't play lands or cast spells from their graveyards this turn"];
}
