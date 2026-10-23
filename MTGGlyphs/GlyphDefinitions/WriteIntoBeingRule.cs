namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Write into Being, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Write into Being</exampledoc>
[TokenizationOrder(-1)]
public class WriteIntoBeingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["manifest one of those cards, then put the other on the top or bottom of your library"];
}
