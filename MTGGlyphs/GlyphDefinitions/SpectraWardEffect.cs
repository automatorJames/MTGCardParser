namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Spectra Ward, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spectra Ward</exampledoc>
[Dependent]
public class SpectraWardEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it can't be blocked, targeted, or dealt damage by anything that's white, blue, black, red, or green"];
}
