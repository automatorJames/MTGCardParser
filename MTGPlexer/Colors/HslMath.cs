using System.Globalization;

namespace MTGPlexer.Colors;

/// <summary>Shared HSL/hex conversion math used by both <see cref="DeterministicPalette"/> (legacy positional/type palettes) and <see cref="SpanColorPalette"/> (per-role span colors).</summary>
internal static class HslMath
{
    public static string ToHex(double h, double s, double l)
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

    public static (double h, double s, double l) FromHex(string hex)
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

    public static bool IsGrayscale(string hex)
    {
        if (string.IsNullOrEmpty(hex) || !hex.StartsWith('#') || hex.Length != 7) return false;
        try
        {
            return hex.AsSpan(1, 2).SequenceEqual(hex.AsSpan(3, 2)) && hex.AsSpan(3, 2).SequenceEqual(hex.AsSpan(5, 2));
        }
        catch { return false; }
    }

    public static string AdjustLightness(string hex, double newLightness)
    {
        var (h, s, _) = FromHex(hex);
        return ToHex(h, s, newLightness);
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
}
