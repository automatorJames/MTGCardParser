using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace MTGPlexer.TokenAnalysis.ColorCoding;

public record DeterministicPalette
{
    static Dictionary<int, Dictionary<int, DeterministicPalette>> _positionalPalettes { get; set; } = [];

    public string Hex { get; private set; }
    public string HexLight { get; private set; }
    public string HexDark { get; private set; }
    public string HexSat { get; private set; } 

    private const double BaseSaturation = 0.9;
    private const double LightSaturation = 0.9;
    private const double DarkSaturation = 0.3;
    private const double SatBoost = 0.3;        

    private const double BaseLightness = 0.6;
    private const double LightLightness = 0.8;
    private const double DarkLightness = 0.3;

    public DeterministicPalette(Type type, double? baseSaturation = null, double? baseLightness = null)
    {
        var colorAttribute = type.GetCustomAttribute<ColorAttribute>();

        if (colorAttribute != null)
            SetFromColor(colorAttribute.Color);
        else
            SetFromSeed(type.Name, baseSaturation, baseLightness);
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

    public static DeterministicPalette GetPositionalPalette(int totalItemCount, int itemPosition)
    {
        Dictionary<int, DeterministicPalette> positionalPalette;

        if (_positionalPalettes.TryGetValue(totalItemCount, out positionalPalette))
            return positionalPalette[itemPosition];
        else
        {
            positionalPalette = [];
            var hues = GetRainbowDivisions(totalItemCount);

            for (int i = 0; i < totalItemCount; i++)
                positionalPalette[i] = new(hues[i]);

            _positionalPalettes[totalItemCount] = positionalPalette;

            return positionalPalette[itemPosition];
        }
    }

    public static Dictionary<int, DeterministicPalette> GetPositionalPalette(int totalItemCount)
    {
        Dictionary<int, DeterministicPalette> positionalPalette;

        if (_positionalPalettes.TryGetValue(totalItemCount, out positionalPalette))
            return positionalPalette;
        else
        {
            positionalPalette = [];
            var hues = GetRainbowDivisions(totalItemCount);

            for (int i = 0; i < totalItemCount; i++)
                positionalPalette[i] = new(hues[i]);

            _positionalPalettes[totalItemCount] = positionalPalette;

            return positionalPalette;
        }
    }

    void SetFromSeed(string seed, double? baseSaturation = null, double? baseLightness = null)
    {
        baseSaturation ??= BaseSaturation;
        baseLightness ??= BaseLightness;
        int hash = GetDeterministicHash(seed);
        uint unsignedHash = (uint)hash;
        double hue = unsignedHash / (double)uint.MaxValue;
        Hex = HslToHex(hue, baseSaturation.Value, baseLightness.Value);
        SetLightDarkFromHue(hue);
        HexSat = HslToHex(hue, Math.Min(1.0, baseSaturation.Value + SatBoost), baseLightness.Value);
    }

    void SetFromColor(HexColor color)
    {
        Hex = color.Value;

        if (IsGrayscale(Hex))
        {
            HexLight = AdjustLightness(Hex, LightLightness);
            HexDark = AdjustLightness(Hex, DarkLightness);
            HexSat = Hex; // keep grayscale intact
        }
        else
        {
            var hue = HexToHue(Hex);
            SetLightDarkFromHue(hue);

            var (h, s, l) = HexToHsl(Hex);
            HexSat = HslToHex(h, Math.Min(1.0, s + SatBoost), l);
        }
    }

    void SetFromRaindbowIndex(int rainbowIndex)
    {
        var rainbowMember = (RainbowMuted)(rainbowIndex % Enum.GetNames(typeof(RainbowMuted)).Length);
        Hex = rainbowMember.Description();
        var hue = HexToHue(Hex);
        SetLightDarkFromHue(hue);
        HexSat = HslToHex(hue, Math.Min(1.0, BaseSaturation + SatBoost), BaseLightness);
    }

    void SetLightDarkFromHue(double hue)
    {
        HexLight = HslToHex(hue, LightSaturation, LightLightness);
        HexDark = HslToHex(hue, DarkSaturation, DarkLightness);
    }

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
            return false;
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

        byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);

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
        else
            hue = 60 * ((rNorm - gNorm) / delta + 4);

        return hue / 360.0;
    }

    static (double h, double s, double l) HexToHsl(string hex)
    {
        if (hex.StartsWith("#")) hex = hex[1..];
        if (hex.Length != 6)
            throw new ArgumentException("Hex must be 6 characters long.", nameof(hex));

        byte r = byte.Parse(hex[..2], NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);

        double rn = r / 255.0, gn = g / 255.0, bn = b / 255.0;
        double max = Math.Max(rn, Math.Max(gn, bn));
        double min = Math.Min(rn, Math.Min(gn, bn));
        double l = (max + min) / 2.0;

        double h, s;
        if (max == min)
        {
            h = 0; s = 0;
        }
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

    static double HueToRgb(double p, double q, double t)
    {
        if (t < 0) t += 1;
        if (t > 1) t -= 1;
        if (t < 1.0 / 6.0) return p + (q - p) * 6 * t;
        if (t < 1.0 / 2.0) return q;
        if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6;
        return p;
    }

    static string AdjustLightness(string hex, double newLightness)
    {
        return HslToHex(0, 0, newLightness);
    }

    static HexColor[] GetRainbowDivisions(int numberOfItems)
    {
        if (numberOfItems <= 0)
            return [];

        string[] colors = new string[numberOfItems];

        float hueStart = 270f; // violet
        float hueEnd = 0f;     // red
        float hueRange = hueStart - hueEnd;

        float saturation = 0.8f;
        float value = 0.96f;

        for (int i = 0; i < numberOfItems; i++)
        {
            float t = (float)i / numberOfItems;
            float hue = hueStart - t * hueRange;

            Color color = FromHsv(hue, saturation, value);
            colors[i] = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        Color FromHsv(float hue, float saturation, float value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60f)) % 6;
            float f = hue / 60f - MathF.Floor(hue / 60f);

            value *= 255f;
            int v = (int)value;
            int p = (int)(value * (1 - saturation));
            int q = (int)(value * (1 - f * saturation));
            int t = (int)(value * (1 - (1 - f) * saturation));

            return hi switch
            {
                0 => Color.FromArgb(v, t, p),
                1 => Color.FromArgb(q, v, p),
                2 => Color.FromArgb(p, v, t),
                3 => Color.FromArgb(p, q, v),
                4 => Color.FromArgb(t, p, v),
                _ => Color.FromArgb(v, p, q),
            };
        }

        return colors
            .Select(x => new HexColor(x))
            .ToArray();
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
