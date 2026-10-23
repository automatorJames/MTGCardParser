namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Soul Search, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Soul Search</exampledoc>
[TokenizationOrder(-1)]
public class SoulSearchRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the card's mana value is 1 or less, create a 1/1 white and black spirit creature token with flying"];
}
