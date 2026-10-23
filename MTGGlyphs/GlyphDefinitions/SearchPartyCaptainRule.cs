namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Search Party Captain, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Search Party Captain</exampledoc>
[TokenizationOrder(-1)]
public class SearchPartyCaptainRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this spell costs {1} less to cast for each creature you attacked with this turn"];
}
