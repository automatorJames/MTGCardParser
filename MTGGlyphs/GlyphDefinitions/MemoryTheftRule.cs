namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Memory Theft, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Memory Theft</exampledoc>
[TokenizationOrder(-1)]
public class MemoryTheftRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may put a card that has an adventure that player owns from exile into that player's graveyard"];
}
