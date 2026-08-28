using System.Collections.Concurrent;
using System.ComponentModel;

namespace Glyphotype.Colors;

/// <summary>
/// Centralized source of <see cref="HexPalette"/>s: equidistant-hue rainbow sets (positional, by type, or by
/// arbitrary item), plus one-off palettes derived from a fixed <see cref="HexColor"/>. Despite the name, this
/// class no longer promises a "same string in, same hue out" mapping anywhere in its public surface — every
/// method here is either rainbow-positional or color-derived. Purely a static utility; nothing here is
/// per-instance state.
/// </summary>
public static class DeterministicPalette
{
    // --- Static Cache ---
    // ConcurrentDictionary because corpus processing builds many ProcessedLines (and therefore
    // palettes) in parallel across documents; a plain Dictionary would race under that.
    static readonly ConcurrentDictionary<(int Count, double SaturationFactor, double LightnessFactor), Dictionary<int, HexPalette>> _positionalPaletteSets = [];
    static readonly ConcurrentDictionary<int, HexPalette> _fixedRainbowPalettes = [];
    static readonly ConcurrentDictionary<HexColor, HexPalette> _staticColorPalettes = [];

    static Dictionary<Type, HexPalette> _typePaletteSet;
    public static Dictionary<Type, HexPalette> TypePaletteSet => _typePaletteSet ??= GetTypePaletteSet();

    // --- Core Color Generation Constants ---
    const double BaseSaturation = 0.66;
    const double FullSaturation = 1.0;
    const double DarkSaturation = 0.3;
    const double BaseLightness = 0.6;
    const double LightLightness = 0.8;
    const double DarkLightness = 0.3;

    /// <summary>
    /// Ceiling for any lightness a caller-supplied factor scales up to. Past this a hue is
    /// indistinguishable from white, which would defeat the point of a rainbow.
    /// </summary>
    const double MaxLightness = 0.92;

    // --- Static Factories ---

    public static HexPalette GetFixedRainbowPalette(int rainbowIndex)
    {
        var rainbowMember = (RainbowMuted)(rainbowIndex % Enum.GetNames(typeof(RainbowMuted)).Length);

        return _fixedRainbowPalettes.GetOrAdd((int)rainbowMember,
            key => BuildFromColor(new HexColor(((RainbowMuted)key).GetDescription())));
    }

    /// <summary>Builds the full type-to-palette map: types carrying a <see cref="ColorAttribute"/> get that fixed color, then every remaining registered token type gets an equidistant rainbow hue, hash-ordered for stability.</summary>
    static Dictionary<Type, HexPalette> GetTypePaletteSet()
    {
        var allGlyphTypes = GlyphTypeRegistry.GetAllTypesExhaustive()
            .OrderBy(x => GetDeterministicHash(x.Name))
            .ToList();

        var explicitlyColoredTypes = allGlyphTypes
            .Select(t => (Type: t, Attribute: t.GetCustomAttribute<ColorAttribute>()))
            .Where(x => x.Attribute != null)
            .ToList();

        var rainbowTypes = allGlyphTypes.Except(explicitlyColoredTypes.Select(x => x.Type)).ToList();

        var typePaletteSet = new Dictionary<Type, HexPalette>();

        foreach (var (type, attribute) in explicitlyColoredTypes)
            typePaletteSet[type] = GetStaticPalette(attribute.Color);

        var rainbowPalettes = GetPositionalPaletteSet(rainbowTypes.Count);
        for (int i = 0; i < rainbowTypes.Count; i++)
            typePaletteSet[rainbowTypes[i]] = rainbowPalettes[i];

        return typePaletteSet;
    }

    public static void RefreshTypePaletteSet() =>
        _typePaletteSet = GetTypePaletteSet();

    /// <summary>
    /// Equidistant rainbow hues for <paramref name="totalItemCount"/> items, keyed by position.
    /// <paramref name="saturationFactor"/>/<paramref name="lightnessFactor"/> scale the shared base
    /// saturation/lightness, which is how a second rainbow keyed to a different dimension can be
    /// told apart from the first at a glance despite reusing the same hues (see
    /// <see cref="GlyphKeyPaletteKnobs"/>).
    /// </summary>
    public static Dictionary<int, HexPalette> GetPositionalPaletteSet(
        int totalItemCount,
        double saturationFactor = 1.0,
        double lightnessFactor = 1.0) =>
        _positionalPaletteSets.GetOrAdd((totalItemCount, saturationFactor, lightnessFactor), key =>
        {
            var positionalPaletteSet = new Dictionary<int, HexPalette>();
            var hues = GetRainbowHues(key.Count);

            for (int i = 0; i < key.Count; i++)
                positionalPaletteSet[i] = BuildFromHue(hues[i], saturationFactor: key.SaturationFactor, lightnessFactor: key.LightnessFactor);

            return positionalPaletteSet;
        });

    /// <summary>
    /// Returns a dictionary where the keys are the items passed, and the HexPalettes are composed of
    /// base colors that are rainbow-equidistant according to the number of items. If any positional override
    /// colors are provided, these are associated with items first in order before rainbow-equidistant logic is
    /// applies to the remaining items (useful for distinct first N colors, followed by a rainbow spread).
    /// </summary>
    public static Dictionary<T, HexPalette> GetPositionalPaletteSet<T>(IEnumerable<T> items, params HexColor[] positionalOverrideColors)
    {
        var itemsAsList = items.ToList();
        var rainbowCount = itemsAsList.Count - positionalOverrideColors.Length;
        var rainbowPaletteSet = GetPositionalPaletteSet(rainbowCount).Values.ToList();
        var overridePaletteset = positionalOverrideColors.Select(GetStaticPalette);
        var combinedPaletteSet = overridePaletteset.Concat(rainbowPaletteSet);

        return combinedPaletteSet
            .Select((palette, idx) => new { palette, idx })
            .ToDictionary(x => itemsAsList[x.idx], x => x.palette);
    }

    /// <summary>
    /// The item-keyed form of
    /// <see cref="GetPositionalPaletteSet(int, double, double)"/> - same equidistant hues, scaled
    /// off the shared base saturation/lightness so a second rainbow keyed to a different dimension
    /// reads as a distinct signal from the first.
    /// </summary>
    public static Dictionary<T, HexPalette> GetPositionalPaletteSet<T>(
        IEnumerable<T> items,
        double saturationFactor,
        double lightnessFactor)
    {
        var itemsAsList = items.ToList();
        var paletteSet = GetPositionalPaletteSet(itemsAsList.Count, saturationFactor, lightnessFactor);

        return itemsAsList
            .Select((item, idx) => (item, palette: paletteSet[idx]))
            .ToDictionary(x => x.item, x => x.palette);
    }

    public static HexPalette GetStaticPalette(HexColor color) =>
        _staticColorPalettes.GetOrAdd(color, BuildFromColor);

    // --- Palette Construction ---

    /// <summary>The single, authoritative method for generating all color properties from a hue.</summary>
    static HexPalette BuildFromHue(
        double hue,
        double? baseSaturation = null,
        double? baseLightness = null,
        double saturationFactor = 1.0,
        double lightnessFactor = 1.0)
    {
        double saturation = Math.Clamp((baseSaturation ?? BaseSaturation) * saturationFactor, 0, FullSaturation);
        double lightness = Math.Clamp((baseLightness ?? BaseLightness) * lightnessFactor, 0, MaxLightness);
        double lightLightness = Math.Clamp(LightLightness * lightnessFactor, 0, MaxLightness);

        var hex = HslMath.ToHex(hue, saturation, lightness);
        var light = HslMath.ToHex(hue, saturation, lightLightness);
        var dark = HslMath.ToHex(hue, DarkSaturation, DarkLightness);

        // Sat deliberately ignores saturationFactor: it's the "pop" variant a hover swaps to, so it
        // stays fully saturated no matter how softened the resting color is.
        var sat = HslMath.ToHex(hue, FullSaturation, lightness);

        return new(hex, light, dark, sat);
    }

    static HexPalette BuildFromColor(HexColor color)
    {
        var hex = color.Value;

        if (HslMath.IsGrayscale(hex))
        {
            var light = HslMath.AdjustLightness(hex, LightLightness);
            var dark = HslMath.AdjustLightness(hex, DarkLightness);
            var sat = hex; // No change for grayscale

            return new(hex, light, dark, sat);
        }

        // Deconstruct the given color to get its core components, then regenerate all variants from
        // this hue to ensure consistency.
        var (h, _, l) = HslMath.FromHex(hex);
        return BuildFromHue(h, baseLightness: l);
    }

    // --- Hue Utilities ---

    static int GetDeterministicHash(string text)
    {
        unchecked
        {
            const int fnvPrime = 16777619;
            int hash = (int)2166136261;
            foreach (char c in text)
            {
                hash ^= c;
                hash *= fnvPrime;
            }
            return hash;
        }
    }

    public static double HexToHue(string hex) => HslMath.FromHex(hex).h;

    /// <summary>Equidistant hues around the color wheel for <paramref name="numberOfItems"/>, in stable positional order.</summary>
    static double[] GetRainbowHues(int numberOfItems, ColorWheelOptions options = null)
    {
        if (numberOfItems <= 0) return [];

        options ??= new();

        var hues = new double[numberOfItems];

        // Calculate the step: the smaller of (full circle division) or (user-defined limit)
        double stepDeg = Math.Min(360.0 / numberOfItems, options.MaxDegreesPerRotation);

        // Reverse = false: Clockwise (decrementing degrees, e.g., 280 -> 270)
        // Reverse = true: Counter-clockwise (incrementing degrees, e.g., 280 -> 290)
        double directionMultiplier = options.Reverse ? 1.0 : -1.0;

        for (int i = 0; i < numberOfItems; i++)
        {
            // Always starts exactly at StartingDegree for index 0
            double currentDegree = options.StartingDegree + (i * stepDeg * directionMultiplier);

            // Normalize to [0, 1] range.
            hues[i] = ((currentDegree % 360.0) + 360.0) % 360.0 / 360.0;
        }

        return hues;
    }

    public enum RainbowMuted
    {
        [Description("#9d81ba")] Violet,
        [Description("#7b8dcf")] Blue,
        [Description("#5ca9b4")] Teal,
        [Description("#7d9e5b")] Green,
        [Description("#d8a960")] Yellow,
        [Description("#c77e59")] Orange,
        [Description("#b9676f")] Red
    }
}
