namespace MTGPlexer.Colors;

/// <summary>
/// A color's three rendering states, precomputed as hex strings: <see cref="Default"/> (at rest),
/// <see cref="Hi"/> (highlighted), and <see cref="Lo"/> (lowlighted). Replaces the older pattern of calling
/// a method with a variant enum at render time — every consumer just reads the three properties it needs.
/// </summary>
public readonly record struct SpanColorPalette(string Default, string Hi, string Lo)
{
    /// <summary>Emits the trio as CSS custom properties, e.g. <c>--hex: #...; --hex-hi: #...; --hex-lo: #...;</c>.</summary>
    public string CssVariables(string prefix = "hex") =>
        $"--{prefix}: {Default}; --{prefix}-hi: {Hi}; --{prefix}-lo: {Lo};";

    /// <summary>Builds a palette from <see cref="ColorKnobs"/>: Hi pushes saturation/brightness up toward their maximum by <see cref="ColorKnobs.EffectiveSaturationRange"/>/<see cref="ColorKnobs.EffectiveBrightnessRange"/>; Lo pushes them down toward zero by the same fractions.</summary>
    public static SpanColorPalette FromKnobs(ColorKnobs knobs)
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
            Lo: HslMath.ToHex(hue, saturationLo, brightnessLo));
    }

    /// <summary>
    /// Bridges a legacy <see cref="HexPalette"/> (still the currency for CardLinesPage/WordTreesPage, and
    /// for TypeRegexPage's own per-named-group rainbow fallback) into the 3-state model: <see cref="HexPalette.Normal"/>
    /// stays at rest, <see cref="HexPalette.Light"/> becomes Hi, <see cref="HexPalette.Dark"/> becomes Lo.
    /// </summary>
    public static SpanColorPalette FromHexPalette(HexPalette palette) =>
        new(Default: palette.Normal, Hi: palette.Light, Lo: palette.Dark);
}
