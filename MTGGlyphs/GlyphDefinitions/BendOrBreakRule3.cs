namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Bend or Break, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bend or Break</exampledoc>
[TokenizationOrder(-1)]
public class BendOrBreakRule3 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["tap all lands in the other piles"];
}
