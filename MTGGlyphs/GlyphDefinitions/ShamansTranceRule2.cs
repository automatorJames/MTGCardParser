namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Shaman's Trance, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shaman's Trance</exampledoc>
[TokenizationOrder(-1)]
public class ShamansTranceRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may play lands and cast spells from other players' graveyards this turn as though those cards were in your graveyard"];
}
