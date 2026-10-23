namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Seven Dwarves, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Seven Dwarves</exampledoc>
[TokenizationOrder(-1)]
public class SevenDwarvesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["a deck can have up to seven cards named", Nib.This];
}
