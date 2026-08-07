namespace MTGPlexer.Colors;

/// <summary>Centralized fallback values for any <see cref="ColorKnobs"/> field left unspecified, so "default" has exactly one definition instead of being re-guessed at every call site.</summary>
public static class ColorKnobDefaults
{
    /// <summary>Default saturation (0..1) for a span's resting, non-highlighted, non-lowlighted color.</summary>
    public const double Saturation = 0.6;

    /// <summary>Default brightness (0..1) for a span's resting color.</summary>
    public const double Brightness = 0.55;

    /// <summary>Default fraction (0..1) of the way from resting saturation to fully-saturated (Hi) / fully-desaturated (Lo).</summary>
    public const double SaturationRange = 0.6;

    /// <summary>Default fraction (0..1) of the way from resting brightness to maximum (Hi) / minimum (Lo) brightness.</summary>
    public const double BrightnessRange = 0.45;
}

/// <summary>
/// The tunable "knobs" behind one <see cref="SpanColorPalette"/>: a hue, how saturated/bright that hue is
/// at rest, and how far highlighting/lowlighting are allowed to swing saturation and brightness away from
/// rest. Any knob left null falls back to <see cref="ColorKnobDefaults"/>, so a caller who only cares about
/// hue can write <c>new ColorKnobs(210)</c> and get a sensible, centrally-defined look.
/// </summary>
/// <param name="HueDegrees">Hue in degrees (0-360) around the color wheel, or 0 for a caller (e.g. <see cref="MTGPlexer.RegexGeneration.Presentation.SmartSpanColorPanel"/>'s tables) that always overwrites it before use.</param>
/// <param name="Saturation">Resting saturation (0..1), or null for <see cref="ColorKnobDefaults.Saturation"/>.</param>
/// <param name="Brightness">Resting brightness (0..1), or null for <see cref="ColorKnobDefaults.Brightness"/>.</param>
/// <param name="SaturationRange">
/// How far Hi/Lo push saturation away from rest, as a fraction (0..1) of the available headroom in each
/// direction. 1.0 means Hi fully saturates and Lo fully desaturates; 0.0 means highlighting/lowlighting
/// don't affect saturation at all. Null falls back to <see cref="ColorKnobDefaults.SaturationRange"/>.
/// </param>
/// <param name="BrightnessRange">The brightness equivalent of <paramref name="SaturationRange"/>.</param>
public readonly record struct ColorKnobs(
    double HueDegrees = 0,
    double? Saturation = null,
    double? Brightness = null,
    double? SaturationRange = null,
    double? BrightnessRange = null)
{
    public double EffectiveSaturation => Saturation ?? ColorKnobDefaults.Saturation;
    public double EffectiveBrightness => Brightness ?? ColorKnobDefaults.Brightness;
    public double EffectiveSaturationRange => SaturationRange ?? ColorKnobDefaults.SaturationRange;
    public double EffectiveBrightnessRange => BrightnessRange ?? ColorKnobDefaults.BrightnessRange;

    /// <summary>The hue normalized to the [0,1) fraction that <see cref="HslMath"/> expects.</summary>
    public double HueFraction => ((HueDegrees % 360.0) + 360.0) % 360.0 / 360.0;

    /// <summary>Returns a copy of these knobs with <see cref="HueDegrees"/> replaced, keeping every other knob as-is.</summary>
    public ColorKnobs WithHueDegrees(double hueDegrees) => this with { HueDegrees = hueDegrees };
}
