namespace MTGPlexer.TokenAnalysis;

public record TypeColorPalette
{
    public string Hex { get; private set; }
    public string HexLight { get; private set; }
    public string HexDark { get; private set; }

    private const double BaseSaturation = 0.9;
    private const double LightSaturation = 0.9;
    private const double DarkSaturation = 0.3;

    private const double BaseLightness = 0.7;
    private const double LightLightness = 0.85;
    private const double DarkLightness = 0.3;

    public TypeColorPalette(Type type)
    {
        SetHexesForType(type);
    }

    static readonly Dictionary<Type, string> FixedHexes = new()
    {
        [typeof(Punctuation)] = HslToHex(0, 0, 0.6),
        [typeof(Parenthetical)] = HslToHex(0, 0, 0.4),
    };

    void SetHexesForType(Type type)
    {
        if (FixedHexes.TryGetValue(type, out string baseHex))
        {
            Hex = baseHex;
            HexLight = AdjustLightness(baseHex, LightLightness);
            HexDark = AdjustLightness(baseHex, DarkLightness);
        }
        else
        {
            int hash = GetDeterministicHash(type.Name);
            double hue = Math.Abs(hash) % 360 / 360.0;

            Hex = HslToHex(hue, BaseSaturation, BaseLightness);
            HexLight = HslToHex(hue, LightSaturation, LightLightness);
            HexDark = HslToHex(hue, DarkSaturation, DarkLightness);
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
        // Reuse hue and saturation if needed — for now this is only used for grayscale
        return HslToHex(0, 0, newLightness);
    }
}