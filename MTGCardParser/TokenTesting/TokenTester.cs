using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using MTGCardParser.Data;
using Superpower.Model;

namespace MTGCardParser.TokenTesting;

public class TokenTester
{
    public AggregateCardAnalysis AggregateCardAnalysis { get; set; }

    // Configuration
    readonly string _outputDir;
    readonly List<Card> _cards;
    private readonly bool _omitCapturedTextSegmentProperties = true;

    readonly List<Type> _tokenUnitTypes;
    readonly Dictionary<Type, Color> _typeColors = new();

    private static readonly IReadOnlyList<string> _propertyCaptureColors = new List<string> { "#9d81ba", "#7b8dcf", "#5ca9b4", "#7d9e5b", "#d8a960", "#c77e59", "#b9676f", "#8f8f8f" }.AsReadOnly();

    public TokenTester(int? maxSetSequence = null, bool ignoreEmptyText = true)
    {
        _outputDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MTG_Parser_Analysis");
        Directory.CreateDirectory(_outputDir);
        _cards = DataGetter.GetCards(maxSetSequence, ignoreEmptyText: ignoreEmptyText);
        _tokenUnitTypes = TokenClassRegistry.AppliedOrderTypes.OrderBy(t => t.Name).ToList();

        for (int i = 0; i < _tokenUnitTypes.Count; i++)
        {
            var type = _tokenUnitTypes[i];
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

    private static IEnumerable<CaptureProp> GetOrderedPropertiesFromTemplate(RegexTemplate template)
    {
        if (template == null) yield break;

        foreach (var segment in template.RegexSegments)
        {
            if (segment is IPropRegexSegment propSegment)
            {
                yield return propSegment.CaptureProp;
            }
            else if (segment is TokenCaptureAlternativeSet alternativeSet)
            {
                foreach (var altPropSegment in alternativeSet.Alternatives)
                {
                    yield return altPropSegment.CaptureProp;
                }
            }
        }
    }

    private void RenderTokenUnitDetails(StringBuilder sb, object instance, string captureId, IReadOnlyDictionary<string, string> propertyColorMap)
    {
        var instanceType = instance.GetType();
        var allProperties = instanceType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                                        .Where(p => p.Name != "RegexTemplate" && p.CanRead)
                                        .ToDictionary(p => p.Name);

        var orderedPropNames = GetOrderedPropertiesFromTemplate((instance as ITokenUnit)?.GetRegexTemplate())
            .Select(p => p.Name)
            .Distinct()
            .ToList();

        foreach (var propName in allProperties.Keys)
        {
            if (!orderedPropNames.Contains(propName))
            {
                orderedPropNames.Add(propName);
            }
        }

        var scalarProperties = new List<PropertyInfo>();
        var childTokenUnitProperties = new List<PropertyInfo>();

        foreach (var propName in orderedPropNames)
        {
            if (!allProperties.TryGetValue(propName, out var prop)) continue;

            if (typeof(ITokenUnit).IsAssignableFrom(prop.PropertyType))
            {
                childTokenUnitProperties.Add(prop);
            }
            else
            {
                scalarProperties.Add(prop);
            }
        }

        var visibleScalarProps = new List<(PropertyInfo prop, object value)>();
        foreach (var prop in scalarProperties)
        {
            if (_omitCapturedTextSegmentProperties && prop.PropertyType == typeof(CapturedTextSegment))
            {
                continue;
            }

            var value = prop.GetValue(instance);
            if (value != null && prop.PropertyType.IsValueType && value.Equals(Activator.CreateInstance(prop.PropertyType)))
            {
                continue;
            }
            visibleScalarProps.Add((prop, value));
        }

        if (visibleScalarProps.Any())
        {
            sb.Append("<table><thead><tr><th>Property</th><th>Type</th><th>Value</th></tr></thead><tbody>");
            foreach (var (prop, value) in visibleScalarProps)
            {
                string propColor = propertyColorMap.GetValueOrDefault(prop.Name, "inherit");
                sb.Append($"<tr data-property-name=\"{prop.Name}\" data-capture-id=\"{captureId}\">");
                sb.Append($"<td><span style=\"color: {propColor}; font-weight: bold;\">{HtmlReportGenerator.Encode(prop.Name)}</span></td>");

                string valueClass = GetValueCssClass(prop.PropertyType);
                sb.Append($"<td class=\"{valueClass}\">{HtmlReportGenerator.Encode(GetFriendlyTypeName(prop.PropertyType))}</td>");

                if (value == null || (value is string s && string.IsNullOrEmpty(s)))
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

        foreach (var prop in childTokenUnitProperties)
        {
            var value = prop.GetValue(instance);
            if (value != null)
            {
                sb.Append("<div class=\"property-child-block\">");
                string propColor = propertyColorMap.GetValueOrDefault(prop.Name, "inherit");
                sb.Append($"<h5 data-property-name=\"{prop.Name}\" data-capture-id=\"{captureId}\" style=\"color: {propColor};\">{HtmlReportGenerator.Encode(prop.Name)}</h5>");
                RenderTokenUnitDetails(sb, value, captureId, propertyColorMap);
                sb.Append("</div>");
            }
        }
    }

    //private string BuildNestedEffectHtml(
    //ITokenUnit effect,
    //string effectText,
    //object cardId,
    //int lineIndex,
    //IReadOnlyDictionary<string, string> propertyColorMap,
    //ref int captureIdCounter,
    //StringBuilder detailsHtmlBuilder)
    //{
    //
    //    // Assign a unique ID for this effect instance
    //    string captureId = $"capture-{cardId}-{lineIndex}-{captureIdCounter}";
    //    captureIdCounter++;
    //    var instanceType = effect.GetType();
    //    var template = effect.GetRegexTemplate();
    //
    //    // A segment is a part of the text that needs special rendering.
    //    // Store the final HTML for the segment directly, avoiding lambdas.
    //    var segments = new List<(int Index, int Length, string HtmlContent)>();
    //
    //    var tokenCaptureProps = template.AllUnwrappedTokenCaptureSegments;
    //
    //    for (int i = 0; i < tokenCaptureProps.Count; i++)
    //    {
    //        TokenCaptureSegment propCaptureSegment = tokenCaptureProps[i];
    //        var propValue = propCaptureSegment.CaptureProp.Prop.GetValue(effect);
    //        if (propValue == null) continue;
    //
    //        var propCaptureText = propCaptureSegment.Regex.Match(effectText).Value;
    //        if (propValue is ITokenUnit childTokenUnit)
    //        {
    //            // Recursively build the HTML for the child token
    //            var segmentHtml = BuildNestedEffectHtml(childTokenUnit, propCaptureText, cardId, lineIndex, propertyColorMap, ref captureIdCounter, detailsHtmlBuilder);
    //            segments.Add((i, propCaptureText.Length, segmentHtml));
    //        }
    //        else
    //        {
    //            continue; // Skip this segment
    //        }
    //
    //    }
    //
    //    // 2. Build the final HTML string by assembling the segments.
    //    var inlineHtmlBuilder = new StringBuilder();
    //    segments = segments.OrderBy(s => s.Index).ToList();
    //    int currentIndex = 0;
    //
    //    foreach (var (index, length, htmlContent) in segments)
    //    {
    //        if (index > currentIndex)
    //        {
    //            inlineHtmlBuilder.Append(HtmlReportGenerator.Encode(effectText.Substring(currentIndex, index - currentIndex)));
    //        }
    //        if (index < currentIndex) continue;
    //
    //        inlineHtmlBuilder.Append(htmlContent);
    //        currentIndex = index + length;
    //    }
    //
    //    if (currentIndex < effectText.Length)
    //    {
    //        inlineHtmlBuilder.Append(HtmlReportGenerator.Encode(effectText.Substring(currentIndex)));
    //    }
    //
    //    // 3. Generate the details block for the current effect.
    //    string colorHex = ToHex(_typeColors[instanceType]);
    //    detailsHtmlBuilder.Append($"<div class=\"effect-details-block\" data-capture-id=\"{captureId}\"><h4 style=\"color: {colorHex};\">{instanceType.Name}</h4>");
    //    RenderTokenUnitDetails(detailsHtmlBuilder, effect, captureId, propertyColorMap);
    //    detailsHtmlBuilder.Append("</div>");
    //
    //    // 4. Wrap the inline text in its own span and return.
    //    return $"<span class=\"nested-underline\" style=\"--underline-color: {colorHex};\" data-capture-id=\"{captureId}\">{inlineHtmlBuilder.ToString()}</span>";
    //}


    string WrapTokenInStartSpanTag(ITokenUnit token, string textSpan, string captureId)
    {
        const int pixelGapPerUnderline = 4;

        var tag =
            $@"<span class=""nested-underline"" style=""--underline-color: {ToHex(_typeColors[token.GetType()])}; "
            + $@"padding-bottom: {pixelGapPerUnderline * token.GetDeepestChildLevel() + pixelGapPerUnderline}px;"" "
            + $@"data-capture-id=""{captureId}"">";

        tag += HtmlReportGenerator.Encode(textSpan);

        return tag;
    }

    string BuildNestedEffectHtml(
        ITokenUnit token,
        object cardId,
        int lineIndex,
        IReadOnlyDictionary<string, string> propertyColorMap,
        ref int captureIdCounter,
        StringBuilder sb)
    {
        var html = "";

        // Assign a unique ID for this effect instance
        string captureId = $"capture-{cardId}-{lineIndex}-{captureIdCounter}";
        captureIdCounter++;
        var childTokenSpans = new List<string>();
        int currentspanStart = 0;
        int currentSpanEnd = 0;
        var flattenedTokens = token.FlattenTokenTree();

        for (int i = 0; i < flattenedTokens.Count; i++)
        {
            var currentToken = flattenedTokens[i];
            var nexttoken = i == flattenedTokens.Count - 1 ? currentToken : flattenedTokens[i + 1];
            currentspanStart = currentToken.MatchSpan.Position.Absolute;

            if (i == flattenedTokens.Count - 1)
                currentSpanEnd = currentspanStart + token.MatchSpan.Length;
            else
                currentSpanEnd = nexttoken.MatchSpan.Position.Absolute;

            var currentSpanLength = currentSpanEnd - currentspanStart;

            var textspan = token.MatchSpan.Source.Substring(currentspanStart, currentSpanLength);
            html += WrapTokenInStartSpanTag(currentToken, textspan, captureId);

            if (nexttoken.RecursiveDepth <= currentToken.RecursiveDepth)
                html += @"</span>";

            if (i == flattenedTokens.Count - 1)
                for (int j = 0; j < currentToken.RecursiveDepth; j++)
                    html += @"</span>";
        }

        // 3. Generate the details block for the current effect.
        string colorHex = ToHex(_typeColors[token.GetType()]);
        html += ($"<div class=\"effect-details-block\" data-capture-id=\"{captureId}\"><h4 style=\"color: {colorHex};\">{token.GetType().Name}</h4>");
        StringBuilder tokenUnitDetailStringBuilder = new();
        RenderTokenUnitDetails(tokenUnitDetailStringBuilder, token, captureId, propertyColorMap);
        html += tokenUnitDetailStringBuilder.ToString() + "</div>";
        sb.Append(html);
        return sb.ToString();
    }


    void GenerateCardVariableCaptureHtml()
    {
        string htmlContent = HtmlReportGenerator.Generate("Card Variable Capture", sb =>
        {
            foreach (var analyzedCard in AggregateCardAnalysis.AnalyzedCards)
            {
                sb.Append($"<div class=\"card-capture-block\">");
                sb.Append($"<h2>{HtmlReportGenerator.Encode(analyzedCard.Card.Name)}</h2>");
                sb.Append($"<pre class=\"full-original-text\">{HtmlReportGenerator.Encode(analyzedCard.Card.Text)}</pre>");

                for (int i = 0; i < analyzedCard.ProcessedLineTokens.Count; i++)
                {
                    if (i >= analyzedCard.Clauses.Count) continue;

                    var lineTokens = analyzedCard.ProcessedLineTokens[i];
                    var effectsToShow = analyzedCard.Clauses[i].Effects.Where(e => !e.GetType().IsDefined(typeof(IgnoreInAnalysisAttribute))).ToList();
                    if (!effectsToShow.Any()) continue;

                    sb.Append("<div class=\"line-capture-block\">");
                    sb.Append($"<h5 class=\"line-label\">Line #{i + 1}</h5>");

                    var allLineProperties = effectsToShow
                        .SelectMany(eff => GetOrderedPropertiesFromTemplate(eff?.GetRegexTemplate()).Select(p => p.Name))
                        .Distinct().ToList();

                    var propertyColorMap = new Dictionary<string, string>();
                    for (int k = 0; k < allLineProperties.Count; k++)
                    {
                        propertyColorMap[allLineProperties[k]] = _propertyCaptureColors[k % _propertyCaptureColors.Count];
                    }

                    var lineTextBuilder = new StringBuilder();
                    var detailsBuilder = new StringBuilder();
                    int captureIdCounter = 0; // Reset for each line to keep IDs manageable
                    int effectIndex = 0;

                    foreach (var token in lineTokens)
                    {
                        var tokenText = token.ToStringValue();
                        if (!token.Kind.IsDefined(typeof(IgnoreInAnalysisAttribute)))
                        {
                            var effect = effectsToShow[effectIndex];
                            lineTextBuilder.Append(BuildNestedEffectHtml(effect, analyzedCard.Card.CardId, i, propertyColorMap, ref captureIdCounter, detailsBuilder));
                            effectIndex++;
                        }
                        else
                        {
                            lineTextBuilder.Append(HtmlReportGenerator.Encode(tokenText));
                        }
                        lineTextBuilder.Append(" ");
                    }

                    sb.Append($"<pre class=\"line-text\">{lineTextBuilder.ToString().TrimEnd()}</pre>");
                    sb.Append(detailsBuilder.ToString());

                    sb.Append("</div>");
                }
                sb.Append("</div>");
            }
        }, isVariableCapturePage: true);

        File.WriteAllText(Path.Combine(_outputDir, "Card Variable Capture.html"), htmlContent);
    }


    private string GetValueCssClass(Type type)
    {
        if (type == typeof(CapturedTextSegment)) return "value-tokensegment";
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
            foreach (var type in TokenClassRegistry.AppliedOrderTypes)
            {
                if (!AggregateCardAnalysis.TokenCaptureCounts.ContainsKey(type)) continue;
                string typeName = type.Name;
                int count = AggregateCardAnalysis.TokenCaptureCounts[type];
                string colorHex = ToHex(_typeColors[type]);
                string renderedRegex = TokenClassRegistry.TypeRegexTemplates[type].RenderedRegexString;
                string encodedTypeName = HtmlReportGenerator.Encode(typeName);
                sb.Append($"<div class=\"type-card\" style=\"border-left-color: {colorHex};\">");
                sb.Append("<h3>");
                sb.Append($"<span class=\"highlight\" data-title=\"{encodedTypeName}\" style=\"background-color: {colorHex};\">{encodedTypeName}</span>");
                sb.Append($" {count} occurrences");
                sb.Append("</h3>");
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
                            if (token.Kind != typeof(Punctuation)) sb.Append($"<span class=\"highlight-label\" style=\"color: {colorHex};\">{typeName}</span>");
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
                double coverage = totalChars > 0 ? matchedChars * 100.0 / totalChars : 0.0;
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
    static string ToHex(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";
}