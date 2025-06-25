using MTGCardParser.TokenCaptures;
using System.Text;

namespace MTGCardParser;

public class TokenTester
{
    public AggregateCardAnalysis AggregateCardAnalysis { get; set; }

    // Configuration
    readonly string _outputDir;
    readonly List<Card> _cards;

    readonly List<Type> _tokenCaptureTypes;
    readonly Dictionary<Type, Color> _typeColors = new();


    public TokenTester(int? maxSetSequence = null, bool ignoreEmptyText = true)
    {
        _outputDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MTG_Parser_Analysis");
        Directory.CreateDirectory(_outputDir);
        _cards = DataGetter.GetCards(maxSetSequence, ignoreEmptyText: ignoreEmptyText);
        _tokenCaptureTypes = TokenCaptureFactory.AppliedOrderTypes.OrderBy(t => t.Name).ToList();

        for (int i = 0; i < _tokenCaptureTypes.Count; i++)
        {
            var type = _tokenCaptureTypes[i];
            _typeColors[type] = GenerateColorForTypeName(type.Name);
        }
    }

    public void Process()
    {
        AggregateCardAnalysis = new(_cards);

        Console.WriteLine("\nGenerating HTML reports...");
        GenerateTypeKeyHtml();
        GenerateCardCoverageHtml();
        GenerateUnmatchedSpansHtml();
        Console.WriteLine($"HTML reports generated successfully in: {_outputDir}");
    }

    void GenerateTypeKeyHtml()
    {
        string htmlContent = HtmlReportGenerator.Generate("Token Type Key & Regex Analysis", sb =>
        {
            foreach (var type in TokenCaptureFactory.AppliedOrderTypes)
            {
                if (!AggregateCardAnalysis.TokenCaptureCounts.ContainsKey(type)) continue;

                string typeName = type.Name;
                int count = AggregateCardAnalysis.TokenCaptureCounts[type];
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

    void GenerateCardCoverageHtml()
    {
        string htmlContent = HtmlReportGenerator.Generate("Card Coverage Analysis", sb =>
        {
            sb.Append("<table><thead><tr><th>Card Name</th><th>Text</th><th>Coverage %</th></tr></thead><tbody>");

            foreach (var analyzedCard in AggregateCardAnalysis.AnalyzedCards)
            {
                sb.Append("<tr>");
                sb.Append($"<td>{HtmlReportGenerator.Encode(analyzedCard.Card.Name)}</td>");
                sb.Append("<td>");

                int matchedChars = 0;
                int totalChars = 0;

                string cumulativeCardText = "";
                bool lastSpanEndedInTermination = true; // starts as true to capitalize first letter;

                for (int i = 0; i < analyzedCard.ProcessedLineTokens.Count; i++)
                {
                    var tokens = analyzedCard.ProcessedLineTokens[i];

                    for (int j = 0; j < tokens.Count; j++)
                    {
                        var token = tokens[j];
                        string textValue = token.Span.ToStringValue();

                        textValue = textValue.Replace(Card.ThisToken, analyzedCard.Card.Name);

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

                        if (token.Kind != typeof(string))
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

    void GenerateUnmatchedSpansHtml()
    {
        string htmlContent = HtmlReportGenerator.Generate("Unmatched Spans Report", sb =>
        {
            sb.Append("<table><thead><tr><th>Count</th><th>Span Text</th><th>First Card</th><th>First Card Text</th></tr></thead><tbody>");

            var sortedSpans = AggregateCardAnalysis.UnmatchedSegmentSpans
                .OrderByDescending(kv => kv.Value.Count)
                .ThenBy(kv => kv.Key.ToStringValue())
                .ToList();

            foreach (var item in sortedSpans)
            {
                sb.Append("<tr>");
                sb.Append($"<td>{item.Value.Count}</td>");
                sb.Append($"<td>{HtmlReportGenerator.Encode(item.Key.ToString())}</td>");
                sb.Append($"<td>{HtmlReportGenerator.Encode(item.Value.FirstRepresentativeCard.Card.Name)}</td>");
                sb.Append("<td>");

                // Use the stored TextSpan for precise highlighting.
                // The TextSpan's source is the line it was found in.
                string lineSource = item.Value.FirstRepresentativeCardOccurrence.Source;

                var firstCardText = item.Value.FirstRepresentativeCard.Card.Text;

                // Find where this specific line starts within the full card text.
                int lineStartIndex = firstCardText.IndexOf(lineSource, StringComparison.Ordinal);

                // If the line is found, calculate the absolute position of the span.
                if (lineStartIndex != -1)
                {
                    int spanStartInLine = item.Value.FirstRepresentativeCardOccurrence.Position.Absolute;
                    int spanLength = item.Value.FirstRepresentativeCardOccurrence.Length;
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
                    int index = firstCardText.IndexOf(item.Key.ToStringValue(), StringComparison.OrdinalIgnoreCase);
                    if (index == -1)
                    {
                        sb.Append(HtmlReportGenerator.Encode(firstCardText).Replace("\n", "<br>"));
                    }
                    else
                    {
                        string pre = HtmlReportGenerator.Encode(firstCardText.Substring(0, index));
                        string match = HtmlReportGenerator.Encode(firstCardText.Substring(index, item.Key.ToStringValue().Length));
                        string post = HtmlReportGenerator.Encode(firstCardText.Substring(index + item.Key.ToStringValue().Length));

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

    static Color GenerateColorForTypeName(string name)
    {
        int hash = GetDeterministicHash(name);
        double hue = (Math.Abs(hash) % 360) / 360.0;
        return HslToRgb(hue, 0.9, 0.7);
    }
    static Color HslToRgb(double h, double s, double l)
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

    static double HueToRgb(double p, double q, double t)
    {
        if (t < 0) t += 1;
        if (t > 1) t -= 1;
        if (t < 1.0 / 6.0) return p + (q - p) * 6 * t;
        if (t < 1.0 / 2.0) return q;
        if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6;
        return p;
    }

    static string ToHex(Color c) =>
        $"#{c.R:X2}{c.G:X2}{c.B:X2}";
}