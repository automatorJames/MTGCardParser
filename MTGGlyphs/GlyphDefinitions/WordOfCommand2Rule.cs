namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Word of Command, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Word of Command</exampledoc>
[TokenizationOrder(-1)]
public class WordOfCommand2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the chosen card is cast as a spell, you control the player while that spell is resolving"];
}
