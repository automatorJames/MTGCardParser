namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Magnetic Theft, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Magnetic Theft</exampledoc>
[Dependent]
public class MagneticTheftEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["control of the equipment doesn't change"];
}
