// Splice in this updated version of TokenTester.cs

using System.Reflection;
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
            _typeColors[type] = GenerateColorForType(type);
        }
    }

    public void Process(bool hydrateAllTokenInstances = false)
    {
        AggregateCardAnalysis = new(_cards);

        Console.WriteLine("\nGenerating HTML reports...");
        GenerateTypeKeyHtml();
        GenerateCardCoverageHtml();
        GenerateUnmatchedSpansHtml();

        if (hydrateAllTokenInstances)
        {
            Console.WriteLine("Hydrating token instances for detailed capture report...");
            foreach (var card in AggregateCardAnalysis.AnalyzedCards)
                card.SetClauseEffects();

            GenerateCardVariableCaptureHtml();
        }

        Console.WriteLine($"HTML reports generated successfully in: {_outputDir}");
    }

    void GenerateCardVariableCaptureHtml()
    {
        string htmlContent = HtmlReportGenerator.Generate("Card Variable Capture", sb =>
        {
            foreach (var analyzedCard in AggregateCardAnalysis.AnalyzedCards)
            {
                sb.Append($"<div class=\"card-capture-block\">");
                sb.Append($"<h2>{HtmlReportGenerator.Encode(analyzedCard.Card.Name)}</h2>");

                // Show full original text once at the top
                sb.Append($"<pre class=\"full-original-text\">{HtmlReportGenerator.Encode(analyzedCard.Card.Text)}</pre>");

                for (int i = 0; i < analyzedCard.ProcessedLineTokens.Count; i++)
                {
                    if (i >= analyzedCard.Clauses.Count) continue;

                    var lineTokens = analyzedCard.ProcessedLineTokens[i];
                    var effectsToShow = analyzedCard.Clauses[i].Effects.Where(e => e.GetType() != typeof(Punctuation)).ToList();

                    sb.Append("<div class=\"line-capture-block\">");
                    sb.Append($"<h5 class=\"line-label\">Line #{i + 1}</h5>");

                    // Render the tokenized line with underlines
                    sb.Append($"<pre class=\"line-text\">");
                    int effectIndex = 0;
                    foreach (var token in lineTokens)
                    {
                        var tokenText = HtmlReportGenerator.Encode(token.ToStringValue());
                        if (token.Kind != typeof(string) && token.Kind != typeof(Punctuation))
                        {
                            string colorHex = ToHex(_typeColors[token.Kind]);
                            string captureId = $"capture-{analyzedCard.Card.CardId}-{i}-{effectIndex}";
                            sb.Append($"<span class=\"captured-text\" style=\"border-bottom-color: {colorHex};\" data-capture-id=\"{captureId}\">{tokenText}</span>");
                            effectIndex++;
                        }
                        else { sb.Append(tokenText); }
                        sb.Append(" ");
                    }
                    sb.Append("</pre>");

                    // Render the details for each effect
                    for (int j = 0; j < effectsToShow.Count; j++)
                    {
                        var effect = effectsToShow[j];
                        var effectType = effect.GetType();
                        string colorHex = ToHex(_typeColors[effectType]);
                        string captureId = $"capture-{analyzedCard.Card.CardId}-{i}-{j}";

                        sb.Append($"<div class=\"effect-details-block\" data-capture-id=\"{captureId}\">");
                        sb.Append($"<h4 style=\"color: {colorHex};\">{effectType.Name}</h4>");

                        var properties = effectType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                                                   .Where(p => p.Name != "RegexTemplate").ToList();

                        if (properties.Any())
                        {
                            sb.Append("<table><thead><tr><th>Property</th><th>Type</th><th>Value</th></tr></thead><tbody>");
                            foreach (var prop in properties)
                            {
                                var propType = prop.PropertyType;
                                var value = prop.GetValue(effect);

                                if (value == null)
                                {
                                    sb.Append($"<tr><td>{HtmlReportGenerator.Encode(prop.Name)}</td><td>{HtmlReportGenerator.Encode(GetFriendlyTypeName(propType))}</td><td class=\"value-empty\">(empty)</td></tr>");
                                    continue;
                                }
                                if (propType.IsValueType && value.Equals(Activator.CreateInstance(propType))) continue;

                                sb.Append("<tr>");
                                sb.Append($"<td>{HtmlReportGenerator.Encode(prop.Name)}</td>");

                                string valueClass = GetValueCssClass(propType);
                                sb.Append($"<td class=\"{valueClass}\">{HtmlReportGenerator.Encode(GetFriendlyTypeName(propType))}</td>");

                                if (value is string s && string.IsNullOrEmpty(s))
                                {
                                    sb.Append($"<td class=\"value-empty\">(empty)</td>");
                                }
                                else
                                {
                                    sb.Append($"<td class=\"{valueClass}\">{HtmlReportGenerator.Encode(value.ToString())}</td>");
                                }
                                sb.Append("</tr>");
                            }
                            sb.Append("</tbody></table>");
                        }
                        sb.Append("</div>");
                    }
                    sb.Append("</div>");
                }
                sb.Append("</div>");
            }
        }, isVariableCapturePage: true);

        File.WriteAllText(Path.Combine(_outputDir, "Card Variable Capture.html"), htmlContent);
    }

    private string GetValueCssClass(Type type)
    {
        if (type == typeof(TokenSegment)) return "value-tokensegment";
        bool isEnum = type.IsEnum || (Nullable.GetUnderlyingType(type)?.IsEnum ?? false);
        if (isEnum) return "value-enum";
        return "value-default";
    }

    private string GetFriendlyTypeName(Type type)
    {
        bool isNullableEnum = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && type.GetGenericArguments()[0].IsEnum;
        if (type.IsEnum || isNullableEnum) return "Enum";
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) return $"{type.GetGenericArguments()[0].Name}?";
        return type.Name;
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
                string encodedTypeName = HtmlReportGenerator.Encode(typeName);
                sb.Append($"<div class=\"type-card\" style=\"border-left-color: {colorHex};\">");
                sb.Append("<h3>");
                sb.Append($"<span class=\"highlight\" data-title=\"{encodedTypeName}\" style=\"background-color: {colorHex};\">{encodedTypeName}</span>");
                sb.Append($" {count} occurrences");
                sb.Append("</h3>");
                sb.Append("<h4>Regex Template</h4>");
                sb.Append($"<pre><code>{HtmlReportGenerator.Encode(regexTemplate)}</code></pre>");
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
            sb.Append("<table><thead><tr><th>Card Name</th><th>Text</th><th>Coverage</th></tr></thead><tbody>");
            foreach (var analyzedCard in AggregateCardAnalysis.AnalyzedCards)
            {
                sb.Append("<tr>");
                sb.Append($"<td>{HtmlReportGenerator.Encode(analyzedCard.Card.Name)}</td>");
                sb.Append($"<td data-original-text=\"{HtmlReportGenerator.Encode(analyzedCard.Card.Text)}\">");
                int matchedChars = 0;
                int totalChars = 0;
                bool lastSpanEndedInTermination = true;
                for (int i = 0; i < analyzedCard.ProcessedLineTokens.Count; i++)
                {
                    var tokens = analyzedCard.ProcessedLineTokens[i];
                    for (int j = 0; j < tokens.Count; j++)
                    {
                        var token = tokens[j];
                        string textValue = token.Span.ToStringValue().Replace(Card.ThisToken, analyzedCard.Card.Name);
                        if (i > 0 && j == 0) { sb.Append("<br><br>"); lastSpanEndedInTermination = true; }
                        if (lastSpanEndedInTermination && textValue.Length > 0) textValue = char.ToUpper(textValue[0]) + textValue.Substring(1);
                        lastSpanEndedInTermination = textValue.Replace("\"", "").EndsWith(".");
                        string encodedText = HtmlReportGenerator.Encode(textValue);
                        int charCount = textValue.Replace("\n", "").Length;
                        if (token.Kind != typeof(string))
                        {
                            string colorHex = ToHex(_typeColors[token.Kind]);
                            string typeName = HtmlReportGenerator.Encode(token.Kind.Name);
                            sb.Append($"<span class=\"highlight\" data-title=\"{typeName}\" style=\"background-color: {colorHex};\">");
                            if (token.Kind.Name != "Punctuation") sb.Append($"<span class=\"highlight-label\" style=\"color: {colorHex};\">{typeName}</span>");
                            sb.Append(encodedText);
                            sb.Append("</span>");
                            matchedChars += charCount;
                        }
                        else { sb.Append(encodedText); }
                        totalChars += charCount;
                        if (j < tokens.Count - 1) { sb.Append(" "); totalChars++; }
                    }
                }
                sb.Append("</td>");
                double coverage = totalChars > 0 ? (matchedChars * 100.0 / totalChars) : 0.0;
                sb.Append($"<td>{coverage:F1}%</td>");
                sb.Append("</tr>");
            }
            sb.Append("</tbody></table>");
        }, isCardCoveragePage: true);
        File.WriteAllText(Path.Combine(_outputDir, "Card Coverage.html"), htmlContent);
    }

    void GenerateUnmatchedSpansHtml()
    {
        string htmlContent = HtmlReportGenerator.Generate("Unmatched Spans Report", sb =>
        {
            sb.Append("<table><thead><tr><th>Count</th><th>Span Text</th><th>First Card</th><th>First Card Text</th></tr></thead><tbody>");
            var sortedSpans = AggregateCardAnalysis.UnmatchedSegmentSpans.OrderByDescending(kv => kv.Value.Count).ThenBy(kv => kv.Key.ToStringValue()).ToList();
            foreach (var item in sortedSpans)
            {
                sb.Append("<tr>");
                sb.Append($"<td>{item.Value.Count}</td>");
                sb.Append($"<td>{HtmlReportGenerator.Encode(item.Key.ToString())}</td>");
                sb.Append($"<td>{HtmlReportGenerator.Encode(item.Value.FirstRepresentativeCard.Card.Name)}</td>");
                sb.Append("<td>");
                string lineSource = item.Value.FirstRepresentativeCardOccurrence.Source;
                var firstCardText = item.Value.FirstRepresentativeCard.Card.Text;
                int lineStartIndex = firstCardText.IndexOf(lineSource, StringComparison.Ordinal);
                if (lineStartIndex != -1)
                {
                    int spanStartInLine = item.Value.FirstRepresentativeCardOccurrence.Position.Absolute;
                    int spanLength = item.Value.FirstRepresentativeCardOccurrence.Length;
                    int absoluteStartIndex = lineStartIndex + spanStartInLine;
                    if (absoluteStartIndex + spanLength <= firstCardText.Length)
                    {
                        string pre = HtmlReportGenerator.Encode(firstCardText.Substring(0, absoluteStartIndex));
                        string match = HtmlReportGenerator.Encode(firstCardText.Substring(absoluteStartIndex, spanLength));
                        string post = HtmlReportGenerator.Encode(firstCardText.Substring(absoluteStartIndex + spanLength));
                        sb.Append(pre.Replace("\n", "<br>"));
                        sb.Append($"<span class=\"unmatched-highlight\">{match}</span>");
                        sb.Append(post.Replace("\n", "<br>"));
                    }
                    else { sb.Append(HtmlReportGenerator.Encode(firstCardText).Replace("\n", "<br>")); }
                }
                else
                {
                    int index = firstCardText.IndexOf(item.Key.ToStringValue(), StringComparison.OrdinalIgnoreCase);
                    if (index == -1) { sb.Append(HtmlReportGenerator.Encode(firstCardText).Replace("\n", "<br>")); }
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
        unchecked { const int fnvPrime = 16777619; int hash = (int)2166136261; foreach (char c in text) { hash ^= c; hash *= fnvPrime; } return hash; }
    }

    static Color GenerateColorForType(Type type)
    {
        if (type == typeof(Punctuation))
            return HslToRgb(0, 0, 0.6);

        int hash = GetDeterministicHash(type.Name);
        double hue = (Math.Abs(hash) % 360) / 360.0;
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
    static string ToHex(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";
}