namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Word of Command, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Word of Command</exampledoc>
[TokenizationOrder(-1)]
public class WordOfCommandCard4Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the player plays that card if able"];
}
