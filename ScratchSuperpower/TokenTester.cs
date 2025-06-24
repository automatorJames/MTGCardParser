using MTGCardParser.TokenCaptures;
using System.Text;

namespace MTGCardParser;

public class TokenTester
{
    // Configuration
    private readonly List<Card> _cards;
    private readonly string _outputDir;

    // Data collection
    private readonly Dictionary<string, (int Count, string FirstCard, string FirstCardText, TextSpan FirstOccurrence)> _unmatchedSpans = new(StringComparer.OrdinalIgnoreCase);
    private int _totalUnmatchedTokens;
    private int _totalPlaceholderTokens;
    private readonly List<Type> _tokenCaptureTypes;
    private readonly string _debugCardName;
    private readonly Dictionary<Type, int> _tokenCaptureCounts = new();
    private readonly Dictionary<Type, Color> _typeColors = new();

    // NEW: A blacklist of types to exclude from highlighting in the coverage report.
    private static readonly List<Type> HighlightBlacklist = new()
    {
        typeof(Period)
    };

    public TokenTester(int? maxSetSequence = null, bool ignoreEmptyText = true, string debugCardName = null)
    {
        _debugCardName = debugCardName;
        _outputDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MTG_Parser_Analysis");
        Directory.CreateDirectory(_outputDir);

        _cards = DataGetter.GetCards(maxSetSequence, ignoreEmptyText: ignoreEmptyText);

        _tokenCaptureTypes = TokenCaptureFactory.AppliedOrderTypes.OrderBy(t => t.Name).ToList();

        for (int i = 0; i < _tokenCaptureTypes.Count; i++)
        {
            var type = _tokenCaptureTypes[i];
            _typeColors[type] = GenerateColorForTypeName(type.Name);
            _tokenCaptureCounts[type] = 0;
        }
    }

    public void Process()
    {
        foreach (var card in _cards)
        {
            foreach (var line in card.CleanedLines)
            {
                var lineTokens = TokenCaptureFactory.Tokenize(line).ToList();
                card.ProcessedLineTokens.Add(lineTokens);

                if (card.Name == _debugCardName)
                    DebugCardTokens(card, lineTokens);
            }

            AnalyzeTokens(card);
        }

        Console.WriteLine("\nGenerating HTML reports...");
        GenerateTypeKeyHtml();
        GenerateCardCoverageHtml();
        GenerateUnmatchedSpansHtml();
        Console.WriteLine($"HTML reports generated successfully in: {_outputDir}");
    }

    private void DebugCardTokens(Card card, List<Token<Type>> tokens)
    {
        Console.WriteLine(card.Name + "\n");
        Console.WriteLine(card.Text + "\n");

        foreach (Token<Type> token in tokens)
            Console.WriteLine(token.Kind.Name + "\n" + token.Span.ToStringValue());
    }

    private void AnalyzeTokens(Card card)
    {
        // The buffer now holds the complete token to preserve its TextSpan.
        var unmatchedBuffer = new List<Token<Type>>();

        foreach (var lineTokenSet in card.ProcessedLineTokens)
        {
            foreach (var token in lineTokenSet)
            {
                // Assuming `typeof(string)` is your "unmatched" token kind.
                if (token.Kind == typeof(string))
                {
                    _totalUnmatchedTokens++;
                    unmatchedBuffer.Add(token); // Add the entire token to the buffer.
                }
                else
                {
                    // A matched token was found, so flush any preceding unmatched tokens.
                    FlushUnmatchedBuffer(card, unmatchedBuffer);
                    unmatchedBuffer.Clear();

                    // Process the matched token.
                    if (token.Kind == typeof(Placeholder))
                        _totalPlaceholderTokens++;
                    else if (_tokenCaptureCounts.ContainsKey(token.Kind))
                        _tokenCaptureCounts[token.Kind]++;
                }
            }

            // Flush any remaining unmatched tokens at the end of the card's text.
            FlushUnmatchedBuffer(card, unmatchedBuffer);
            unmatchedBuffer.Clear(); // Clear for the next card.
        }
    }

    private void FlushUnmatchedBuffer(Card card, List<Token<Type>> buffer)
    {
        if (buffer.Count == 0) return;

        // Create a new TextSpan that covers the entire sequence of unmatched tokens.
        var startSpan = buffer.First().Span;
        var endSpan = buffer.Last().Span;
        var combinedSpan = startSpan.Until(endSpan);

        // Use the text from the combined span as the dictionary key.
        var spanText = combinedSpan.ToStringValue().Trim();
        if (string.IsNullOrEmpty(spanText)) return;

        if (_unmatchedSpans.TryGetValue(spanText, out var info))
        {
            // If this text has been seen before, just increment the count.
            _unmatchedSpans[spanText] = (info.Count + 1, info.FirstCard, info.FirstCardText, info.FirstOccurrence);
        }
        else
        {
            // This is the first time we've seen this text; store it with its positional data.
            _unmatchedSpans[spanText] = (1, card.Name, card.Text, combinedSpan);
        }
    }

    #region HTML Generation Methods

    private void GenerateTypeKeyHtml()
    {
        string htmlContent = HtmlReportGenerator.Generate("Token Type Key & Regex Analysis", sb =>
        {
            foreach (var type in TokenCaptureFactory.AppliedOrderTypes)
            {
                if (!_tokenCaptureCounts.ContainsKey(type)) continue;

                string typeName = type.Name;
                int count = _tokenCaptureCounts[type];
                string colorHex = ToHex(_typeColors[type]);
                string regexTemplate = TokenCaptureFactory.GetRegexTemplate(type);
                string renderedRegex = TokenCaptureFactory.GetRenderedRegex(type);

                sb.Append($"<div class=\"type-card\" style=\"border-left-color: {colorHex};\">");

                // Header with Type Name (highlighted) and Count
                sb.Append("<h3>");
                sb.Append($"<span class=\"highlight\" style=\"background-color: {colorHex};\">{HtmlReportGenerator.Encode(typeName)}</span>");
                sb.Append($" {count} occurrences");
                sb.Append("</h3>");

                // Regex Template
                sb.Append("<h4>Regex Template</h4>");
                sb.Append($"<pre><code>{HtmlReportGenerator.Encode(regexTemplate)}</code></pre>");

                // Rendered Regex
                sb.Append("<h4>Rendered Regex</h4>");
                sb.Append($"<pre><code>{HtmlReportGenerator.Encode(renderedRegex)}</code></pre>");

                sb.Append("</div>");
            }
        });

        File.WriteAllText(Path.Combine(_outputDir, "Type Key.html"), htmlContent);
    }

    private void GenerateCardCoverageHtml()
    {
        string htmlContent = HtmlReportGenerator.Generate("Card Coverage Analysis", sb =>
        {
            sb.Append("<table><thead><tr><th>Card Name</th><th>Text</th><th>Coverage %</th></tr></thead><tbody>");

            foreach (var card in _cards)
            {
                sb.Append("<tr>");
                sb.Append($"<td>{HtmlReportGenerator.Encode(card.Name)}</td>");
                sb.Append("<td>");

                int matchedChars = 0;
                int totalChars = 0;

                string cumulativeCardText = "";
                bool lastSpanEndedInTermination = true; // starts as true to capitalize first letter;

                for (int i = 0; i < card.ProcessedLineTokens.Count; i++)
                {
                    var tokens = card.ProcessedLineTokens[i];

                    for (int j = 0; j < tokens.Count; j++)
                    {
                        var token = tokens[j];
                        string textValue = token.Span.ToStringValue();

                        textValue = textValue.Replace(Card.ThisToken, card.Name);

                        // line break
                        if (i > 0 && j == 0)
                        {
                            sb.Append("<br><br>");
                            lastSpanEndedInTermination = true;
                        }

                        if (lastSpanEndedInTermination && textValue.Length > 0)
                            textValue = textValue.Substring(0, 1).ToUpper() + textValue.Substring(1);

                        cumulativeCardText += " " + textValue;

                        lastSpanEndedInTermination = textValue.Replace("\"", "").EndsWith(".");

                        string encodedText = HtmlReportGenerator.Encode(textValue);
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
                            sb.Append($"<span class=\"highlight\" data-title=\"{typeName}\" style=\"background-color: {colorHex};\">{encodedText}</span>");
                            matchedChars += charCount;
                        }
                        else
                        {
                            sb.Append(encodedText);
                        }

                        totalChars += charCount;
                        if (j < tokens.Count - 1)
                        {
                            sb.Append(" ");
                            totalChars++;
                        }
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

            foreach (var (span, (count, firstCard, firstCardText, firstOccurrence)) in sortedSpans)
            {
                sb.Append("<tr>");
                sb.Append($"<td>{count}</td>");
                sb.Append($"<td>{HtmlReportGenerator.Encode(span)}</td>");
                sb.Append($"<td>{HtmlReportGenerator.Encode(firstCard)}</td>");
                sb.Append("<td>");

                // Use the stored TextSpan for precise highlighting.
                // The TextSpan's source is the line it was found in.
                string lineSource = firstOccurrence.Source;
                // Find where this specific line starts within the full card text.
                int lineStartIndex = firstCardText.IndexOf(lineSource, StringComparison.Ordinal);

                // If the line is found, calculate the absolute position of the span.
                if (lineStartIndex != -1)
                {
                    int spanStartInLine = firstOccurrence.Position.Absolute;
                    int spanLength = firstOccurrence.Length;
                    int absoluteStartIndex = lineStartIndex + spanStartInLine;

                    // Ensure the calculated position is within bounds.
                    if (absoluteStartIndex + spanLength <= firstCardText.Length)
                    {
                        string pre = HtmlReportGenerator.Encode(firstCardText.Substring(0, absoluteStartIndex));
                        string match = HtmlReportGenerator.Encode(firstCardText.Substring(absoluteStartIndex, spanLength));
                        string post = HtmlReportGenerator.Encode(firstCardText.Substring(absoluteStartIndex + spanLength));

                        sb.Append(pre.Replace("\n", "<br>"));
                        sb.Append($"<span class=\"unmatched-highlight\">{match}</span>");
                        sb.Append(post.Replace("\n", "<br>"));
                    }
                    else
                    {
                        // Fallback for safety, though this case should be rare.
                        sb.Append(HtmlReportGenerator.Encode(firstCardText).Replace("\n", "<br>"));
                    }
                }
                else
                {
                    // Fallback: if the original line can't be found, highlight the first string match.
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
                }

                sb.Append("</td>");
                sb.Append("</tr>");
            }

            sb.Append("</tbody></table>");
        });

        File.WriteAllText(Path.Combine(_outputDir, "Unmatched Spans.html"), htmlContent);
    }

    #endregion

    #region Color Generation (Unchanged)

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

    private static Color GenerateColorForTypeName(string name)
    {
        int hash = GetDeterministicHash(name);
        double hue = (Math.Abs(hash) % 360) / 360.0;
        return HslToRgb(hue, 0.9, 0.7);
    }
    private static Color HslToRgb(double h, double s, double l)
    {
        double r, g, b;

        if (s == 0)
        {
            r = g = b = l; // achromatic
        }
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

    private static double HueToRgb(double p, double q, double t)
    {
        if (t < 0) t += 1;
        if (t > 1) t -= 1;
        if (t < 1.0 / 6.0) return p + (q - p) * 6 * t;
        if (t < 1.0 / 2.0) return q;
        if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6;
        return p;
    }

    private static string ToHex(Color c) =>
        $"#{c.R:X2}{c.G:X2}{c.B:X2}";
    #endregion
}