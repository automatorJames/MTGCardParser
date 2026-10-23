namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Wan Shi Tong, Librarian, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wan Shi Tong, Librarian</exampledoc>
[TokenizationOrder(-1)]
public class WanShiTongLibrarianCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then draw half x cards, rounded down"];
}
