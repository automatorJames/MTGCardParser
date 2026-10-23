namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Library of Leng, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Library of Leng</exampledoc>
/// <examplecapture>if an effect causes you to discard a card, discard it, but you may put it on top of your library instead of into your graveyard</examplecapture>
[TokenizationOrder(-1)]
public class LibraryOfLengRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if an effect causes you to discard a card, discard it, but you may put it on top of your library instead of into your graveyard"];
}
