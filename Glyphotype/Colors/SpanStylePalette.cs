namespace Glyphotype.Colors;

/// <summary>
/// A span's full rendering treatment: a color's three states, precomputed as hex strings —
/// <see cref="Default"/> (at rest), <see cref="Hi"/> (highlighted), and <see cref="Lo"/> (lowlighted) —
/// plus the non-color style flags (<see cref="IsBold"/>, <see cref="IsItalic"/>) that round it out into a
/// general-purpose span style, not just a color. Deliberately excludes any per-span font-family override:
/// mixing font families within one formatted regex breaks the fixed-column-width math the whole rendering
/// pipeline depends on (see <see cref="Glyphotype.RegexGeneration.Presentation.SmartFontConfig"/>). Replaces
/// the older pattern of calling a method with a variant enum at render time — every consumer just reads the
/// properties it needs.
/// </summary>
public readonly record struct SpanStylePalette(string Default, string Hi, string Lo, bool IsBold = false, bool IsItalic = false)
{
    /// <summary>Emits the color trio as CSS custom properties, e.g. <c>--hex: #...; --hex-hi: #...; --hex-lo: #...;</c>.</summary>
    public string CssVariables(string prefix = "hex") =>
        $"--{prefix}: {Default}; --{prefix}-hi: {Hi}; --{prefix}-lo: {Lo};";

    /// <summary>Builds a palette from <see cref="ColorKnobs"/>: Hi pushes saturation/brightness up toward their maximum by <see cref="ColorKnobs.EffectiveSaturationRange"/>/<see cref="ColorKnobs.EffectiveBrightnessRange"/>; Lo pushes them down toward zero by the same fractions. The style flags carry straight across from <paramref name="knobs"/>.</summary>
    public static SpanStylePalette FromKnobs(ColorKnobs knobs)
    {
        var hue = knobs.HueFraction;
        var saturation = knobs.EffectiveSaturation;
        var brightness = knobs.EffectiveBrightness;

        var saturationHi = saturation + (1 - saturation) * knobs.EffectiveSaturationRange;
        var saturationLo = saturation - saturation * knobs.EffectiveSaturationRange;
        var brightnessHi = brightness + (1 - brightness) * knobs.EffectiveBrightnessRange;
        var brightnessLo = brightness - brightness * knobs.EffectiveBrightnessRange;

        return new(
            Default: HslMath.ToHex(hue, saturation, brightness),
            Hi: HslMath.ToHex(hue, saturationHi, brightnessHi),
            Lo: HslMath.ToHex(hue, saturationLo, brightnessLo),
            IsBold: knobs.IsBold,
            IsItalic: knobs.IsItalic);
    }

    /// <summary>
    /// Bridges a legacy <see cref="HexPalette"/> (still the currency for CardLinesPage/WordTreesPage, and
    /// for TypeRegexPage's own per-named-group rainbow fallback) into the 3-state model: <see cref="HexPalette.Normal"/>
    /// stays at rest, <see cref="HexPalette.Light"/> becomes Hi, <see cref="HexPalette.Dark"/> becomes Lo.
    /// Carries no style flags — <see cref="HexPalette"/>-sourced spans have never had bold/italic treatment.
    /// </summary>
    public static SpanStylePalette FromHexPalette(HexPalette palette) =>
        new(Default: palette.Normal, Hi: palette.Light, Lo: palette.Dark);
}
