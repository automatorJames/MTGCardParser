namespace MTGCardParser.Static;

public class CardTextAnalyzer
{
    public AggregateCardAnalysis AggregateCardAnalysis { get; set; }
    public Dictionary<Type, Color> TypeColors { get; set; } = new();
    public List<string> PropertyCaptureColors { get; set; } = ["#9d81ba", "#7b8dcf", "#5ca9b4", "#7d9e5b", "#d8a960", "#c77e59", "#b9676f", "#8f8f8f"];

    public CardTextAnalyzer(int? maxSetSequence = null, bool ignoreEmptyText = true)    
    {
        var cards = DataGetter.GetCards(maxSetSequence, ignoreEmptyText: ignoreEmptyText);
        var tokenUnitTypes = TokenClassRegistry.AppliedOrderTypes.OrderBy(t => t.Name).ToList();

        for (int i = 0; i < tokenUnitTypes.Count; i++)
        {
            var type = tokenUnitTypes[i];
            TypeColors[type] = GenerateColorForType(type);
        }

        AggregateCardAnalysis = new(cards);
    }

    public static int GetDeterministicHash(string text)
    {
        unchecked { const int fnvPrime = 16777619; int hash = (int)2166136261; foreach (char c in text) { hash ^= c; hash *= fnvPrime; } return hash; }
    }

    static Color GenerateColorForType(Type type)
    {
        if (type == typeof(Punctuation))
            return HslToRgb(0, 0, 0.6);

        else if (type == typeof(Parenthetical))
            return HslToRgb(0, 0, 0.4);

        int hash = GetDeterministicHash(type.Name);
        double hue = Math.Abs(hash) % 360 / 360.0;
        return HslToRgb(hue, 0.9, 0.7);
    }

    static Color HslToRgb(double h, double s, double l)
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
        return Color.FromArgb(255, (int)(r * 255), (int)(g * 255), (int)(b * 255));
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

    public string ToHex(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";
}

