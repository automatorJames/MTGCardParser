namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Kav Landseeker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kav Landseeker</exampledoc>
[Dependent]
public class KavLandseekerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["sacrifice that token"];
}
