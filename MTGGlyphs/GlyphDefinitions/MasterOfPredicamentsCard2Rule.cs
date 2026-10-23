namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Master of Predicaments, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Master of Predicaments</exampledoc>
[TokenizationOrder(-1)]
public class MasterOfPredicamentsCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player guesses whether the card's mana value is greater than 4"];
}
