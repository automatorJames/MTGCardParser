namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Martyr of Sands, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Martyr of Sands</exampledoc>
[Dependent]
public class MartyrOfSandsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain three times x life"];
}
