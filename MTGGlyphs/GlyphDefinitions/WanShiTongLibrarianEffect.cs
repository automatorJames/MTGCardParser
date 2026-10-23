namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Wan Shi Tong, Librarian, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wan Shi Tong, Librarian</exampledoc>
[Dependent]
public class WanShiTongLibrarianEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter on wan shi tong and draw a card"];
}
