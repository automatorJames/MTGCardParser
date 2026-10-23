namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sudden Setback, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sudden Setback</exampledoc>
[TokenizationOrder(-1)]
public class SuddenSetbackRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the owner of target spell or nonland permanent puts it on their choice of the top or bottom of their library"];
}
