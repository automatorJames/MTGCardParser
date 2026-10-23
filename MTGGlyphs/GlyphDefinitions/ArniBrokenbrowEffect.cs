namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Arni Brokenbrow, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Arni Brokenbrow</exampledoc>
[Dependent]
public class ArniBrokenbrowEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may have arni's base power become 1 plus the greatest power among other creatures you control until end of turn"];
}
