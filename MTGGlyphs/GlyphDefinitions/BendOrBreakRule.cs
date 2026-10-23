namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Bend or Break, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bend or Break</exampledoc>
[TokenizationOrder(-1)]
public class BendOrBreakRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each player, one of their piles is chosen by one of their opponents of their choice"];
}
