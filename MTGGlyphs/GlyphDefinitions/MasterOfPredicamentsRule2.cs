namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Master of Predicaments, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Master of Predicaments</exampledoc>
[TokenizationOrder(-1)]
public class MasterOfPredicamentsRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the player guessed wrong, you may cast the card without paying its mana cost"];
}
