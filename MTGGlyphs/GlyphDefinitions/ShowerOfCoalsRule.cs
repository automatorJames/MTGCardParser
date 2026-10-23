namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Shower of Coals, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shower of Coals</exampledoc>
[TokenizationOrder(-1)]
public class ShowerOfCoalsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["threshold —", Nib.This, "deals 4 damage to each of those permanents and/or players instead if there are seven or more cards in your graveyard"];
}
