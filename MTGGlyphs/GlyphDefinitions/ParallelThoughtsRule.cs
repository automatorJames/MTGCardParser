namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Parallel Thoughts, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Parallel Thoughts</exampledoc>
[TokenizationOrder(-1)]
public class ParallelThoughtsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you would draw a card, you may instead put the top card of the pile you exiled into your hand"];
}
