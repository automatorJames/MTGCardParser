namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Nashi, Searcher in the Dark, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nashi, Searcher in the Dark</exampledoc>
[TokenizationOrder(-1)]
public class NashiSearcherInTheDarkRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you put no cards into your hand this way, put a +1/+1 counter on nashi"];
}
