namespace MTGPlexer.TokenTesting;

public class AggregateCardAnalysis
{
    public List<CardAnalysis> AnalyzedCards = new();
    public Dictionary<TextSpan, UnmatchedSpanOccurrence> UnmatchedSegmentSpans { get; set; } = new(new TextSpanAsStringComparer());
    public Dictionary<Type, int> TokenCaptureCounts { get; set; } = new();
    public Dictionary<Type, string> TypeColors { get; set; } = new();
    public List<string> PropertyCaptureColors { get; set; } = ["#9d81ba", "#7b8dcf", "#5ca9b4", "#7d9e5b", "#d8a960", "#c77e59", "#b9676f", "#8f8f8f"];
    public int TotalUnmatchedTokens { get; set; }

    public AggregateCardAnalysis(int? maxSetSequence = null, bool ignoreEmptyText = true)
    {
        var cards = DataGetter.GetCards(maxSetSequence, ignoreEmptyText: ignoreEmptyText);
        var tokenUnitTypes = TokenClassRegistry.AppliedOrderTypes.OrderBy(t => t.Name).ToList();

        for (int i = 0; i < tokenUnitTypes.Count; i++)
        {
            var type = tokenUnitTypes[i];
            TypeColors[type] = GenerateColorHexForType(type);
        }

        Process(cards);
    }

    public void Process(List<Card> cards)
    {
        foreach (var type in TokenClassRegistry.AppliedOrderTypes.OrderBy(x => x.Name))
            TokenCaptureCounts[type] = 0;

        foreach (Card card in cards)
        {
            var analyzedCard = new CardAnalysis(card);
            analyzedCard.AddAccumulatedCapturedTokenTypeCounts(TokenCaptureCounts);

            TotalUnmatchedTokens += analyzedCard.UnmatchedTokenCount;

            foreach (var unmatchedSegmentSpan in analyzedCard.UnmatchedSegmentSpans)
                if (UnmatchedSegmentSpans.ContainsKey(unmatchedSegmentSpan))
                    UnmatchedSegmentSpans[unmatchedSegmentSpan]++;
                else
                    UnmatchedSegmentSpans[unmatchedSegmentSpan] = new UnmatchedSpanOccurrence(analyzedCard, unmatchedSegmentSpan);

            AnalyzedCards.Add(analyzedCard);
        }

        foreach (var card in AnalyzedCards)
            card.SetCapturedLines();
    }

    public static int GetDeterministicHash(string text)
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

    static string GenerateColorHexForType(Type type)
    {
        if (type == typeof(Punctuation))
            return HslToHex(0, 0, 0.6);
        else if (type == typeof(Parenthetical))
            return HslToHex(0, 0, 0.4);

        int hash = GetDeterministicHash(type.Name);
        double hue = Math.Abs(hash) % 360 / 360.0;
        return HslToHex(hue, 0.9, 0.7);
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
}
