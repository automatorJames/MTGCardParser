namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Unexpected Results, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Unexpected Results</exampledoc>
[TokenizationOrder(-1)]
public class UnexpectedResultsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["shuffle your library, then reveal the top card"];
}
