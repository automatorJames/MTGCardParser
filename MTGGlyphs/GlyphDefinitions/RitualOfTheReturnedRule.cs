namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ritual of the Returned, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ritual of the Returned</exampledoc>
[TokenizationOrder(-1)]
public class RitualOfTheReturnedRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["its power is equal to that card's power and its toughness is equal to that card's toughness"];
}
