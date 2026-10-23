namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Lunar Whale, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Lunar Whale</exampledoc>
[TokenizationOrder(-1)]
public class TheLunarWhaleRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as", Nib.This, "attacked this turn, you may play the top card of your library"];
}
