using System.ComponentModel;
using System.Globalization;

namespace MTGPlexer.Colors;

public record DeterministicPalette
{
    // --- Static Cache ---
    static readonly Dictionary<int, Dictionary<int, HexPalette>> _positionalPaletteSets = [];
    static readonly Dictionary<int, HexPalette> _fixedRainbowPalettes = [];
    static readonly Dictionary<HexColor, HexPalette> _staticColorPalettes = [];

    static Dictionary<Type, HexPalette> _typePaletteSet;
    public static Dictionary<Type, HexPalette> TypePaletteSet
    {
        get
        {
            if (_typePaletteSet == null)
                _typePaletteSet = GetTypePaletteSet();

            return _typePaletteSet;
        }
    }

    // --- Public Color Properties ---
    public HexPalette Palette { get; private set; }


    /// <summary>
    /// The string used to generate this palette, typically a token Type name.
    /// </summary>
    public string Seed { get; private set; }

    // --- Core Color Generation Constants ---
    const double BaseSaturation = 0.66;
    const double FullSaturation = 1.0;
    const double DarkSaturation = 0.3;
    const double BaseLightness = 0.6;
    const double LightLightness = 0.8;
    const double DarkLightness = 0.3;

    // --- Constructors (Public Signatures Unchanged) ---

    public DeterministicPalette(Type type, double? baseSaturation = null, double? baseLightness = null)
    {
        Seed = type.Name.ToFriendlyCase(TitleDisplayOption.Title);
        var colorAttribute = type.GetCustomAttribute<ColorAttribute>();
        if (colorAttribute != null)
            InitializeFromColor(colorAttribute.Color);
        else
            InitializeFromHue(GetHueFromSeed(type.Name), baseSaturation, baseLightness);
    }

    public DeterministicPalette(string seed)
    {
        Seed = seed;
        InitializeFromHue(GetHueFromSeed(seed));
    }

    public DeterministicPalette(HexColor color)
    {
        InitializeFromColor(color);
    }

    public DeterministicPalette(int rainbowIndex)
    {
        var rainbowMember = (RainbowMuted)(rainbowIndex % Enum.GetNames(typeof(RainbowMuted)).Length);
        InitializeFromColor(new HexColor(rainbowMember.Description()));
    }

    // Private constructor for direct, consistent hue initialization.
    private DeterministicPalette(double hue)
    {
        InitializeFromHue(hue);
    }

    // --- Static Factory ---

    public static HexPalette GetFixedRainbowPalette(int rainbowIndex)
    {
        var rainbowMember = (RainbowMuted)(rainbowIndex % Enum.GetNames(typeof(RainbowMuted)).Length);

        if (_fixedRainbowPalettes.TryGetValue((int)rainbowMember, out var palette))
            return palette;

        var newPalette = new DeterministicPalette(rainbowIndex).Palette;
        _fixedRainbowPalettes[(int)rainbowMember] = newPalette;

        return newPalette;
    }

    static Dictionary<Type, HexPalette> GetTypePaletteSet()
    {
        if (_typePaletteSet != null)
            return _typePaletteSet;

        _typePaletteSet = [];

        var allTokenTypes = TokenTypeRegistry.NameToType.Values
            .OrderBy(x => GetDeterministicHash(x.Name))
            .ToList();

        var positionalPalettes = GetPositionalPaletteSet(allTokenTypes.Count);

        for (int i = 0; i < allTokenTypes.Count; i++)
            _typePaletteSet[allTokenTypes[i]] = positionalPalettes[i];

        return _typePaletteSet;
    }

    public static Dictionary<int, HexPalette> GetPositionalPaletteSet(int totalItemCount)
    {
        if (_positionalPaletteSets.TryGetValue(totalItemCount, out var positionalPaletteSet))
        {
            return positionalPaletteSet;
        }

        positionalPaletteSet = [];
        var hues = GetRainbowHues(totalItemCount);

        for (int i = 0; i < totalItemCount; i++)
        {
            // Use the private constructor to create palettes directly and consistently from hues.
            positionalPaletteSet[i] = new DeterministicPalette(hues[i]).Palette;
        }

        _positionalPaletteSets[totalItemCount] = positionalPaletteSet;
        return positionalPaletteSet;
    }

    public static HexPalette GetStaticPalette(HexColor color)
    {
        if (_staticColorPalettes.TryGetValue(color, out var palette))
            return palette;

        var newPalette = new DeterministicPalette(color).Palette;
        _staticColorPalettes[color] = newPalette;

        return newPalette;
    }


    // --- Internal Initializers ---

    /// <summary>
    /// The single, authoritative method for generating all color properties from a hue.
    /// </summary>
    private void InitializeFromHue(double hue, double? baseSaturation = null, double? baseLightness = null)
    {
        double saturation = baseSaturation ?? BaseSaturation;
        double lightness = baseLightness ?? BaseLightness;

        // Create the base Hex color with the standard, slightly reduced saturation.
        var hex = HslToHex(hue, saturation, lightness);

        // Create light and dark variants based on the standard saturation.
        var light = HslToHex(hue, saturation, LightLightness);
        var dark = HslToHex(hue, DarkSaturation, DarkLightness);

        // Create the saturated version with full saturation for highlighting.
        var sat = HslToHex(hue, FullSaturation, lightness);

        Palette = new(hex, light, dark, sat);
    }

    /// <summary>
    /// Initializes all properties based on a pre-existing color.
    /// </summary>
    private void InitializeFromColor(HexColor color)
    {
        var hex = color.Value;

        if (IsGrayscale(hex))
        {
            var light = AdjustLightness(hex, LightLightness);
            var dark = AdjustLightness(hex, DarkLightness);
            var sat = hex; // No change for grayscale

            Palette = new(hex, light, dark, sat);
        }
        else
        {
            // Deconstruct the given color to get its core components.
            var (h, _, l) = HexToHsl(hex);
            // Regenerate all variants from this hue to ensure consistency.
            InitializeFromHue(h, baseLightness: l);
        }
    }


    // --- Color Conversion & Utility Methods ---

    private static double GetHueFromSeed(string seed)
    {
        int hash = GetDeterministicHash(seed);
        uint unsignedHash = (uint)hash;
        return unsignedHash / (double)uint.MaxValue;
    }

    private static bool IsGrayscale(string hex)
    {
        if (string.IsNullOrEmpty(hex) || !hex.StartsWith('#') || hex.Length != 7) return false;
        try
        {
            return hex.AsSpan(1, 2).SequenceEqual(hex.AsSpan(3, 2)) && hex.AsSpan(3, 2).SequenceEqual(hex.AsSpan(5, 2));
        }
        catch { return false; }
    }

    private static int GetDeterministicHash(string text)
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

    private static string HslToHex(double h, double s, double l)
    {
        double r, g, b;
        if (s == 0) { r = g = b = l; }
        else
        {
            double q = l < 0.5 ? l * (1 + s) : l + s - l * s;
            double p = 2 * l - q;
            r = HueToRgb(p, q, h + 1.0 / 3.0);
            g = HueToRgb(p, q, h);
            b = HueToRgb(p, q, h - 1.0 / 3.0);
        }
        return $"#{(int)(r * 255):X2}{(int)(g * 255):X2}{(int)(b * 255):X2}";
    }

    public static double HexToHue(string hex)
    {
        var (h, _, _) = HexToHsl(hex);
        return h;
    }

    private static (double h, double s, double l) HexToHsl(string hex)
    {
        if (hex.StartsWith("#")) hex = hex[1..];
        if (hex.Length != 6) throw new ArgumentException("Hex must be 6 characters long.", nameof(hex));

        byte r = byte.Parse(hex[..2], NumberStyles.HexNumber);
        byte g = byte.Parse(hex.AsSpan(2, 2), NumberStyles.HexNumber);
        byte b = byte.Parse(hex.AsSpan(4, 2), NumberStyles.HexNumber);

        double rn = r / 255.0, gn = g / 255.0, bn = b / 255.0;
        double max = Math.Max(rn, Math.Max(gn, bn));
        double min = Math.Min(rn, Math.Min(gn, bn));
        double l = (max + min) / 2.0;
        double h, s;

        if (max == min) { h = s = 0; }
        else
        {
            double d = max - min;
            s = l > 0.5 ? d / (2.0 - max - min) : d / (max + min);
            if (max == rn) h = (gn - bn) / d + (gn < bn ? 6 : 0);
            else if (max == gn) h = (bn - rn) / d + 2;
            else h = (rn - gn) / d + 4;
            h /= 6.0;
        }
        return (h, s, l);
    }

    private static double HueToRgb(double p, double q, double t)
    {
        if (t < 0) t += 1;
        if (t > 1) t -= 1;
        if (t < 1.0 / 6.0) return p + (q - p) * 6 * t;
        if (t < 1.0 / 2.0) return q;
        if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6;
        return p;
    }

    private static string AdjustLightness(string hex, double newLightness)
    {
        var (h, s, _) = HexToHsl(hex);
        return HslToHex(h, s, newLightness);
    }

    private static double[] GetRainbowHues(int numberOfItems, ColorWheelOptions options = null)
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