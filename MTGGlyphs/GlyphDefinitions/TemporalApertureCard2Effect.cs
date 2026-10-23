namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Temporal Aperture, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Temporal Aperture</exampledoc>
[Dependent]
public class TemporalApertureCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["shuffle your library, then reveal the top card"];
}
