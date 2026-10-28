namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Word of Command, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Word of Command</exampledoc>
[TokenizationOrder(-1)]
public class WordOfCommandRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you control that player until", Nib.This, "finishes resolving"];
}
