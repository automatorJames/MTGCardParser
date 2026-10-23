namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Serum Powder, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Serum Powder</exampledoc>
[Dependent]
public class SerumPowderCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can do this in addition to taking mulligans"];
}
