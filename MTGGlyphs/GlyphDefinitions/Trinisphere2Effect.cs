namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Trinisphere, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Trinisphere</exampledoc>
[Dependent]
public class Trinisphere2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["additional mana in the cost may be paid with any color of mana or colorless mana"];
}
