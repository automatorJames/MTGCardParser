namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Wan Shi Tong, Librarian, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wan Shi Tong, Librarian</exampledoc>
[Dependent]
public class WanShiTongLibrarianCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put x +1/+1 counters on him"];
}
