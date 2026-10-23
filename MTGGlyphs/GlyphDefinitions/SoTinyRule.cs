namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of So Tiny, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>So Tiny</exampledoc>
[TokenizationOrder(-1)]
public class SoTinyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it gets -6/-0 instead as long as its controller has seven or more cards in their graveyard"];
}
