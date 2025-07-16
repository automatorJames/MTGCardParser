using System.ComponentModel;
using System.Globalization;

namespace MTGPlexer.TokenAnalysis;

public record DeterministicPalette
{
    public string Hex { get; private set; }
    public string HexLight { get; private set; }
    public string HexDark { get; private set; }

    private const double BaseSaturation = 0.9;
    private const double LightSaturation = 0.9;
    private const double DarkSaturation = 0.3;

    private const double BaseLightness = 0.65;
    private const double LightLightness = 0.95;
    private const double DarkLightness = 0.3;

    public DeterministicPalette(Type type)
    {
        var colorAttribute = type.GetCustomAttribute<ColorAttribute>();

        if (colorAttribute != null)
            SetFromColor(colorAttribute.Color);
        else
            SetFromSeed(type.Name);
    }

    public DeterministicPalette(string seed)
    {
        SetFromSeed(seed);
    }

    public DeterministicPalette(HexColor color)
    {
        SetFromColor(color);
    }

    public DeterministicPalette(int rainbowIndex)
    {
        SetFromRaindbowIndex(rainbowIndex);
    }

    void SetFromSeed(string seed)
    {
        int hash = GetDeterministicHash(seed);

        // --- FIX ---
        // Treat the signed hash as a full-range unsigned integer.
        uint unsignedHash = (uint)hash;

        // Divide by the maximum possible value to map it to the [0, 1] range.
        // This provides a far more uniform distribution of hues than the modulo operator.
        double hue = unsignedHash / (double)uint.MaxValue;
        // --- END FIX ---

        Hex = HslToHex(hue, BaseSaturation, BaseLightness);
        SetLightDarkFromHue(hue);
    }

    void SetFromColor(HexColor color)
    {
        Hex = color.Value;

        // Check if the provided color is grayscale first.
        if (IsGrayscale(Hex))
        {
            // If it is, use the dedicated AdjustLightness logic which forces saturation to 0.
            HexLight = AdjustLightness(Hex, LightLightness);
            HexDark = AdjustLightness(Hex, DarkLightness);
        }
        else
        {
            // Otherwise, proceed with the normal hue-based calculation.
            var hue = HexToHue(Hex);
            SetLightDarkFromHue(hue);
        }
    }

    void SetFromRaindbowIndex(int rainbowIndex)
    {
        var rainbowMember = (RainbowMuted)(rainbowIndex % Enum.GetNames(typeof(RainbowMuted)).Length);
        Hex = rainbowMember.Description();
        var hue = HexToHue(Hex);
        SetLightDarkFromHue(hue);
    }

    void SetLightDarkFromHue(double hue)
    {
        HexLight = HslToHex(hue, LightSaturation, LightLightness);
        HexDark = HslToHex(hue, DarkSaturation, DarkLightness);
    }

    // --- NEW HELPER METHOD ---
    /// <summary>
    /// Determines if a hex color is grayscale by checking if its R, G, and B components are equal.
    /// </summary>
    static bool IsGrayscale(string hex)
    {
        if (string.IsNullOrEmpty(hex)) return false;
        if (hex.StartsWith("#")) hex = hex.Substring(1);
        if (hex.Length != 6) return false;

        try
        {
            string r = hex.Substring(0, 2);
            string g = hex.Substring(2, 2);
            string b = hex.Substring(4, 2);
            return r.Equals(g, StringComparison.OrdinalIgnoreCase) && g.Equals(b, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false; // Invalid format
        }
    }

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

    static string HslToHex(double h, double s, double l)
    {
        double r, g, b;
        if (s == 0)
        {
            r = g = b = l;
        }
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
        if (hex.StartsWith("#"))
            hex = hex.Substring(1);

        if (hex.Length != 6)
            throw new ArgumentException("Hex must be 6 characters long.", nameof(hex));

        // Parse RGB components
        byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);

        // Normalize to [0,1]
        double rNorm = r / 255.0;
        double gNorm = g / 255.0;
        double bNorm = b / 255.0;

        double max = Math.Max(rNorm, Math.Max(gNorm, bNorm));
        double min = Math.Min(rNorm, Math.Min(gNorm, bNorm));
        double delta = max - min;

        double hue;
        if (delta == 0)
            hue = 0;
        else if (max == rNorm)
            hue = 60 * (((gNorm - bNorm) / delta + 6) % 6);
        else if (max == gNorm)
            hue = 60 * ((bNorm - rNorm) / delta + 2);
        else // max == bNorm
            hue = 60 * ((rNorm - gNorm) / delta + 4);

        return hue / 360.0;
    }

    static double HueToRgb(double p, double q, double t)
    {
        if (t < 0) t += 1;
        if (t > 1) t -= 1;
        if (t < 1.0 / 6.0) return p + (q - p) * 6 * t;
        if (t < 1.0 / 2.0) return q;
        if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6;
        return p;
    }

    /// <summary>
    /// Creates a grayscale hex color with a specific lightness.
    /// </summary>
    static string AdjustLightness(string hex, double newLightness)
    {
        // This function is perfect. It correctly forces saturation to 0.
        return HslToHex(0, 0, newLightness);
    }

    public enum RainbowMuted
    {
        [Description("#9d81ba")]
        Violet,

        [Description("#7b8dcf")]
        Blue,

        [Description("#5ca9b4")]
        Teal,

        [Description("#7d9e5b")]
        Green,

        [Description("#d8a960")]
        Yellow,

        [Description("#c77e59")]
        Orange,

        [Description("#b9676f")]
        Red
    }
}