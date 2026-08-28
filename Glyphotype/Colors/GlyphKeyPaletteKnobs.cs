namespace Glyphotype.Colors;

/// <summary>
/// The tuning knobs for the *second* rainbow a word tree card renders: the one keyed by top-level
/// <see cref="Glyph"/> type (the captured, bolded sub-spans inside node text, plus the glyph key
/// strip below the tree), as opposed to the document-keyed rainbow above it.
/// <para>
/// Both key strips draw equidistant hues from the same
/// <see cref="DeterministicPalette.GetPositionalPaletteSet(int, double, double)"/> wheel, so hue
/// alone can't tell the reader which of the two signals they're looking at — a green document
/// border and a green glyph border would read as related when they aren't. These factors pull the
/// glyph rainbow off the document rainbow on the other two HSL axes instead: brighter and less
/// saturated, so glyph color reads as a lighter "wash" over text while document color stays the
/// heavier, more saturated signal on node outlines.
/// </para>
/// </summary>
public static class GlyphKeyPaletteKnobs
{
    /// <summary>
    /// Multiplier applied to <see cref="DeterministicPalette"/>'s base saturation when building a
    /// glyph-keyed palette. Below 1 softens the hue so bolded capture text doesn't compete with the
    /// document-keyed node outlines drawn around it.
    /// </summary>
    public const double SaturationFactor = 0.72;

    /// <summary>
    /// Multiplier applied to <see cref="DeterministicPalette"/>'s base and light lightness when
    /// building a glyph-keyed palette. Above 1 lifts the hue toward white, which is what keeps
    /// capture text legible against the dark node fill at the reduced saturation above.
    /// </summary>
    public const double LightnessFactor = 1.2;
}
