using MTGCardParser.TokenCaptures;
using System.Text;

namespace MTGCardParser;

public class TokenTester
{
    // Configuration
    private readonly List<Card> _cards;
    private readonly string _outputDir;

    // Data collection
    private readonly Dictionary<string, (int Count, string FirstCard, string FirstCardText)> _unmatchedSpans = new(StringComparer.OrdinalIgnoreCase);
    private int _totalUnmatchedTokens;
    private int _totalPlaceholderTokens;
    private readonly List<Type> _tokenCaptureTypes;
    private readonly Dictionary<Type, int> _tokenCaptureCounts = new();
    private readonly Dictionary<Type, Color> _typeColors = new();
    private readonly List<(Card Card, List<Token<Type>> Tokens)> _processedCards = new();

    // NEW: A blacklist of types to exclude from highlighting in the coverage report.
    private static readonly List<Type> HighlightBlacklist = new()
    {
        typeof(Newline),
        typeof(Period)
    };

    public TokenTester(int? maxSetSequence = null, bool ignoreEmptyText = true)
    {
        _outputDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MTG_Parser_Analysis");
        Directory.CreateDirectory(_outputDir);

        _cards = DataGetter.GetCards(maxSetSequence, ignoreEmptyText: ignoreEmptyText);

        _tokenCaptureTypes = TokenCaptureFactory.GetTokenCaptureTypes().OrderBy(t => t.Name).ToList();

        for (int i = 0; i < _tokenCaptureTypes.Count; i++)
        {
            var type = _tokenCaptureTypes[i];
            _typeColors[type] = GenerateColorForTypeName(type.Name);
            _tokenCaptureCounts[type] = 0;
        }
    }

    #region Color Generation (Unchanged)
    private static Color GenerateColorForTypeName(string name)
    {
        int hash = name.GetHashCode();
        double hue = (Math.Abs(hash) % 360) / 360.0;
        return HslToRgb(hue, 0.9, 0.7);
    }
    private static Color HslToRgb(double h, double s, double l) { double r, g, b; if (s == 0) r = g = b = l; else { double q = l < 0.5 ? l * (1 + s) : l + s - l * s; double p = 2 * l - q; r = HueToRgb(p, q, h + 1 / 3.0); g = HueToRgb(p, q, h); b = HueToRgb(p, q, h - 1 / 3.0); } return Color.FromArgb(255, (int)(r * 255), (int)(g * 255), (int)(b * 255)); }
    private static double HueToRgb(double p, double q, double t) { if (t < 0) t += 1; if (t > 1) t -= 1; if (t < 1 / 6.0) return p + (q - p) * 6 * t; if (t < 1 / 2.0) return q; if (t < 2 / 3.0) return p + (q - p) * (2 / 3.0 - t) * 6; return p; }
    private static string ToHex(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";
    #endregion

    public void Process()
    {
        foreach (var card in _cards)
        {
            var tokens = TokenCaptureFactory.CleanAndTokenize(card.Text).ToList();
            _processedCards.Add((card, tokens));
            AnalyzeTokens(card, tokens);
        }

        PrintCoverageSummary();
        PrintUnmatchedTable();

        Console.WriteLine("\nGenerating HTML reports...");
        GenerateTypeKeyHtml();
        GenerateCardCoverageHtml();
        GenerateUnmatchedSpansHtml();
        Console.WriteLine($"HTML reports generated successfully in: {_outputDir}");
    }

    private void AnalyzeTokens(Card card, List<Token<Type>> tokens)
    {
        var unmatchedBuffer = new List<string>();
        foreach (var token in tokens)
        {
            // CRITICAL FIX: Use the token's specific value, not the entire source text.
            if (token.Kind == typeof(string))
            {
                _totalUnmatchedTokens++;
                unmatchedBuffer.Add(token.Span.ToStringValue());
            }
            else
            {
                FlushUnmatchedBuffer(card, unmatchedBuffer);
                unmatchedBuffer.Clear();

                if (token.Kind == typeof(Placeholder))
                {
                    _totalPlaceholderTokens++;
                }
                else if (_tokenCaptureCounts.ContainsKey(token.Kind))
                {
                    _tokenCaptureCounts[token.Kind]++;
                }
            }
        }
        FlushUnmatchedBuffer(card, unmatchedBuffer);
    }

    private void FlushUnmatchedBuffer(Card card, List<string> buffer)
    {
        if (buffer.Count == 0) return;
        var span = string.Join(" ", buffer).Trim();
        if (string.IsNullOrEmpty(span)) return;

        if (_unmatchedSpans.TryGetValue(span, out var info))
            _unmatchedSpans[span] = (info.Count + 1, info.FirstCard, info.FirstCardText);
        else
            _unmatchedSpans[span] = (1, card.Name, card.Text);
    }

    #region Console Output Methods
    private void PrintCoverageSummary() { /* ... unchanged ... */ }
    private void PrintUnmatchedTable()
    {
        var results = _unmatchedSpans
            .OrderByDescending(kv => kv.Value.Count)
            .ThenBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase)
            .ToList();

        // REMOVED: The Top X limit is gone.

        Console.WriteLine($"{"Cnt",5}  {"Unmatched Span",-30}  {"First Card",-30}");
        Console.WriteLine(new string('-', 70));

        foreach (var (span, (count, firstCard, _)) in results)
        {
            Console.WriteLine($"{count,5}  {span,-30}  {firstCard,-30}");
        }
    }
    #endregion

    #region HTML Generation Methods

    private void GenerateTypeKeyHtml() { /* ... unchanged ... */ }

    private void GenerateCardCoverageHtml()
    {
        string htmlContent = HtmlReportGenerator.Generate("Card Coverage Analysis", sb =>
        {
            sb.Append("<table><thead><tr><th>Card Name</th><th>Text</th><th>Coverage %</th></tr></thead><tbody>");

            var sortedCards = _processedCards.OrderBy(c => c.Card.Name, StringComparer.OrdinalIgnoreCase);

            foreach (var (card, tokens) in sortedCards)
            {
                sb.Append("<tr>");
                sb.Append($"<td>{HtmlReportGenerator.Encode(card.Name)}</td>");
                sb.Append("<td>");

                int matchedChars = 0;
                int totalChars = 0;

                for (int i = 0; i < tokens.Count; i++)
                {
                    var token = tokens[i];
                    // CRITICAL FIX: Use the token's specific value.
                    string textValue = token.Span.ToStringValue();
                    string encodedText = HtmlReportGenerator.Encode(textValue).Replace("\n", "<br>");
                    int charCount = textValue.Replace("\n", "").Length;

                    // NEW: Check against the blacklist before highlighting.
                    bool isHighlightable = token.Kind != typeof(string)
                                        && token.Kind != typeof(Placeholder)
                                        && !HighlightBlacklist.Contains(token.Kind);

                    if (isHighlightable)
                    {
                        string colorHex = ToHex(_typeColors[token.Kind]);
                        // NEW: Add a `title` attribute for the tooltip.
                        string typeName = HtmlReportGenerator.Encode(token.Kind.Name);
                        sb.Append($"<span class=\"highlight\" title=\"{typeName}\" style=\"background-color: {colorHex};\">{encodedText}</span>");
                        matchedChars += charCount;
                    }
                    else
                    {
                        sb.Append(encodedText);
                    }

                    totalChars += charCount;
                    if (i < tokens.Count - 1 && token.Kind != typeof(Newline) && tokens[i + 1].Kind != typeof(Newline))
                    {
                        sb.Append(" ");
                        totalChars++;
                    }
                }

                sb.Append("</td>");

                double coverage = totalChars > 0 ? (matchedChars * 100.0 / totalChars) : 0.0;
                sb.Append($"<td>{coverage:F1}%</td>");
                sb.Append("</tr>");
            }

            sb.Append("</tbody></table>");
        });

        File.WriteAllText(Path.Combine(_outputDir, "Card Coverage.html"), htmlContent);
    }

    private void GenerateUnmatchedSpansHtml()
    {
        string htmlContent = HtmlReportGenerator.Generate("Unmatched Spans Report", sb =>
        {
            sb.Append("<table><thead><tr><th>Count</th><th>Span Text</th><th>First Card</th><th>First Card Text</th></tr></thead><tbody>");

            var sortedSpans = _unmatchedSpans
                .OrderByDescending(kv => kv.Value.Count)
                .ThenBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase)
                .ToList();

            // REMOVED: The Top X limit is gone.

            foreach (var (span, (count, firstCard, firstCardText)) in sortedSpans)
            {
                sb.Append("<tr>");
                sb.Append($"<td>{count}</td>");
                sb.Append($"<td>{HtmlReportGenerator.Encode(span)}</td>");
                sb.Append($"<td>{HtmlReportGenerator.Encode(firstCard)}</td>");
                sb.Append("<td>");

                // CRITICAL FIX: The logic for finding the span now works correctly because `span`
                // is a small fragment, not the entire card text.
                int index = firstCardText.IndexOf(span, StringComparison.OrdinalIgnoreCase);
                if (index == -1)
                {
                    sb.Append(HtmlReportGenerator.Encode(firstCardText).Replace("\n", "<br>"));
                }
                else
                {
                    string pre = HtmlReportGenerator.Encode(firstCardText.Substring(0, index));
                    string match = HtmlReportGenerator.Encode(firstCardText.Substring(index, span.Length));
                    string post = HtmlReportGenerator.Encode(firstCardText.Substring(index + span.Length));

                    sb.Append(pre.Replace("\n", "<br>"));
                    sb.Append($"<span class=\"unmatched-highlight\">{match}</span>");
                    sb.Append(post.Replace("\n", "<br>"));
                }

                sb.Append("</td>");
                sb.Append("</tr>");
            }

            sb.Append("</tbody></table>");
        });

        File.WriteAllText(Path.Combine(_outputDir, "Unmatched Spans.html"), htmlContent);
    }
    #endregion
}