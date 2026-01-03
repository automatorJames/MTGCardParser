namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

/// <summary>
/// Handles the formatting and presentation of a sequence of regular expression elements.
/// </summary>
public class RegexFormatter
{
    FormattedRegexColoringRules _colors = new();
    FormattedRegexTreatmentRules _treatments = new();
    int _hashSeparatorColumn;
    int _commentBoxLength;
    const string DefaultWhite = "#FFFFFF";
    const int _hashSeparatorPadding = 4;
    const int _boxContentLeftPadding = 1;
    const int _spacesPerIndent = 4;
    const int _spacesPerAlternateIndent = 2;

    // field to hold layout metrics for enum comment boxes
    Dictionary<string, EnumBoxLayoutMetrics> _enumBoxMetrics;

    /// <summary>
    /// Formats a list of RegexElement objects into a human-readable, commented output.
    /// </summary>
    /// <param name="regexElements">The logical regex elements to format.</param>
    /// <param name="boundaryOption">The boundary option to apply.</param>
    /// <param name="synonymData">Data for enriching enum comments with synonym info.</param>
    /// <returns>A list of richly formatted RegexCommentedLine objects.</returns>
    public List<RegexCommentedLine> Format(
        IReadOnlyList<RegexElement> regexElements,
        BoundaryOption boundaryOption,
        List<PropPathSynonymSetContainer> synonymData)
    {
        synonymData ??= [];
        var synonymDataLookup = synonymData.ToDictionary(d => d.ParentPath.PropPath);
        var alternateCounts = new Dictionary<AlternateValueEnum, int>();

        // 1. Expand containers and enrich with variant data
        var expandedAndFilteredElements = new List<RegexElement>();
        foreach (var element in regexElements)
        {
            if (element is AlternateValueEnumContainer enumContainer)   
            {
                if (synonymDataLookup.TryGetValue(enumContainer.NamedPath, out var wrapper))
                {
                    SynonymTrailingSpacer spacerBuffer = null;

                    foreach (var synonymSet in wrapper.SynonymSets.Values)
                    {
                        if (spacerBuffer != null)
                        {
                            expandedAndFilteredElements.Add(spacerBuffer);
                            spacerBuffer = null;
                        }

                        var enumElement = enumContainer.AlternateValueEnums.Single(e => e.CanonicalValue.Equals(synonymSet.CanonicalValue));

                        if (synonymSet.SynonymCounts.Count > 1)
                        {
                            var header = new SynonymSetHeader(enumElement);
                            alternateCounts[header] = synonymSet.TotalCount;
                            expandedAndFilteredElements.Add(header);

                            foreach (var synonym in synonymSet.SynonymCounts)
                            {
                                var synonymElement = new SynonymValueEnum(enumElement, synonymSet.TotalCount, synonym.Key);
                                alternateCounts[synonymElement] = synonym.Value;
                                expandedAndFilteredElements.Add(synonymElement);
                            }

                            spacerBuffer = new SynonymTrailingSpacer(enumElement);
                        }
                        else
                        {
                            enumElement.DisplayOverrideName = synonymSet.SynonymCounts.First().Key;
                            expandedAndFilteredElements.Add(enumElement);
                            alternateCounts[enumElement] = synonymSet.TotalCount;
                        }
                    }

                    if (wrapper.UnrepresentedAlternateCount > 0)
                        expandedAndFilteredElements.Add(new BlankLine(enumContainer.Enclosures) { Comment = $"{wrapper.UnrepresentedAlternateCount} omitted" });
                }
                else
                    expandedAndFilteredElements.Add(new BlankLine(enumContainer.Enclosures) { Comment = $"All {enumContainer.AlternateValueEnums.Count} omitted" });
            }
            else if (element is AlternateValueContainer container)
                expandedAndFilteredElements.AddRange(container.AlternateValues);
            else
                expandedAndFilteredElements.Add(element);
        }

        if (!expandedAndFilteredElements.Any())
            return [];

        // 2. Add blank lines for spacing
        var finalizedLines = new List<RegexElement> { expandedAndFilteredElements[0] };
        for (var i = 1; i < expandedAndFilteredElements.Count; i++)
        {
            var previousLine = expandedAndFilteredElements[i - 1];
            var currentLine = expandedAndFilteredElements[i];
            var pathChanged = currentLine.UniquePath != previousLine.UniquePath;
            if (pathChanged)
            {
                int GetEnclosureEventType(RegexElement line) => line switch
                {
                    GroupOpen or NamedGroupOpen => 1,
                    GroupClose or NamedGroupClose => 2,
                    _ => 0
                };

                var prevEventType = GetEnclosureEventType(previousLine);
                var currentEventType = GetEnclosureEventType(currentLine);

                if (prevEventType != currentEventType || prevEventType == 0)
                {
                    var commonDepth = 0;

                    while (
                        commonDepth < previousLine.Enclosures.Length 
                        && commonDepth < currentLine.Enclosures.Length 
                        && previousLine.Enclosures[commonDepth].Ordinal == currentLine.Enclosures[commonDepth].Ordinal)
                           commonDepth++;

                    finalizedLines.Add(new BlankLine(previousLine.Enclosures.Take(commonDepth).ToArray()));
                }
            }

            finalizedLines.Add(currentLine);
        }

        AddBoundaryLines(finalizedLines, boundaryOption);

        // 3. Prepare for formatting
        CalculateColumnWidths(finalizedLines, alternateCounts);

        // 4. Format the lines and create RegexCommentedLine objects
        var commentedLines = new List<RegexCommentedLine>();
        for (var i = 0; i < finalizedLines.Count; i++)
        {
            var line = finalizedLines[i];
            var regexText = GetFinalRegexString(line);

            var spans = new List<RegexCommentedLineSpan>();

            int indentSpaces;
            if (line is AlternateValue)
            {
                var parentDepth = line.Enclosures.Count(e => e is not RootEnclosure) - 1;
                if (parentDepth < 0)
                    parentDepth = 0;

                const int alternatePrefixWidth = _spacesPerAlternateIndent + 2; // for "  | "
                indentSpaces = (parentDepth * _spacesPerIndent) + alternatePrefixWidth;
            }
            else
                indentSpaces = GetIndentDepth(line) * _spacesPerIndent;

            var indentedRegex = new string(' ', indentSpaces) + regexText;
            var paddedRegex = indentedRegex.PadRight(_hashSeparatorColumn);

            var primaryContentColor = GetPrimaryContentColorForLine(line);
            var primaryContentPalette = DeterministicPalette.GetStaticPalette(new HexColor(primaryContentColor));
            var highlightTreatment = _treatments.GetRegexHighlightTreatment(line);

            var pathForRegexSpan = line.NamedPath;

            if (line is SynonymValueEnum syn)
                pathForRegexSpan = $"{pathForRegexSpan}.{syn.CanonicalValue}";
            else if (line is AlternateValueEnum altValEnum)
                pathForRegexSpan = $"{pathForRegexSpan}.{altValEnum.CanonicalValue}";
            else if (line is AlternateValue alt)
                pathForRegexSpan = $"{pathForRegexSpan}.{alt.CanonicalValue}";

            var relativePath = RegexCommentedLine.GetRelativePath(pathForRegexSpan);

            if (relativePath == null)
                highlightTreatment = SpanHighlightTreatment.None;

            spans.Add(new(paddedRegex, primaryContentPalette, relativePath, highlightTreatment, _treatments.CommentLowlightTreatment));

            var commentPrefix = $"#{new string(' ', _hashSeparatorPadding)}";
            var hashPalette = DeterministicPalette.GetStaticPalette(new HexColor(_colors.HashSeparatorColor));
            spans.Add(new(commentPrefix, hashPalette, null, SpanHighlightTreatment.None, SpanLowlightTreatment.None));

            var commentSpans = GenerateCommentSpans(line, alternateCounts);
            spans.AddRange(commentSpans);
            var commentText = commentPrefix + string.Join("", commentSpans.Select(s => s.SpanText));

            if (line is AlternateValue alternateValue && line is not SynonymSetHeader && line is not SynonymTrailingSpacer)
                commentedLines.Add(new RegexCommentedAlternateLine(paddedRegex, commentText, line.NamedPath, spans, alternateValue));
            else
                commentedLines.Add(new RegexCommentedLine(paddedRegex, commentText, line.NamedPath, spans));
        }

        // 5. Add alternate ("|") prefixes
        var finalResult = new List<RegexCommentedLine>();
        string currentEnclosurePath = null;
        var isFirstInAlternateGroup = true;

        foreach (var line in commentedLines)
        {
            if (line is RegexCommentedAlternateLine altLine)
            {
                if (altLine.EnclosurePath != currentEnclosurePath)
                {
                    currentEnclosurePath = altLine.EnclosurePath;
                    isFirstInAlternateGroup = true;
                }

                var originalPaddedRegex = altLine.Regex;
                var trimmedRegex = originalPaddedRegex.Trim();
                var textStartColumn = originalPaddedRegex.Length - originalPaddedRegex.TrimStart().Length;

                string newIndentedRegex;
                if (isFirstInAlternateGroup)
                {
                    newIndentedRegex = new string(' ', textStartColumn) + trimmedRegex;
                }
                else
                {
                    var prefix = $"{new string(' ', _spacesPerAlternateIndent)}| ";
                    var prefixStartColumn = textStartColumn - prefix.Length;
                    if (prefixStartColumn < 0) prefixStartColumn = 0;

                    newIndentedRegex = new string(' ', prefixStartColumn) + prefix + trimmedRegex;
                }

                var newPaddedRegex = newIndentedRegex.PadRight(_hashSeparatorColumn);
                var newSpans = altLine.Spans.ToList();
                newSpans[0] = newSpans[0] with { SpanText = newPaddedRegex };
                finalResult.Add(altLine with { Regex = newPaddedRegex, FormattedText = newPaddedRegex + altLine.Comment, Spans = newSpans });
                isFirstInAlternateGroup = false;
            }
            else
            {
                currentEnclosurePath = null;
                isFirstInAlternateGroup = true;
                finalResult.Add(line);
            }
        }

        return finalResult;
    }

    /// <summary>
    /// Adds start and end boundary elements to a list of regex lines based on the specified boundary option.
    /// </summary>
    /// <param name="lines">The list of elements to add boundaries to.</param>
    /// <param name="boundaryOption">The type of boundary to add.</param>
    public static void AddBoundaryLines(List<RegexElement> lines, BoundaryOption boundaryOption)
    {
        if (boundaryOption == BoundaryOption.Omit)
            return;

        RegexElement startBoundary = boundaryOption == BoundaryOption.WholeWord ? new NegativeLookbehindBoundary() : new StartOfLineBoundary();
        RegexElement endBoundary = boundaryOption == BoundaryOption.WholeWord ? new NegativeLookaheadBoundary() : new EndOfLineBoundary();
        lines.Insert(0, startBoundary);
        lines.Insert(1, new BlankLine([]));
        lines.Add(new BlankLine([]));
        lines.Add(endBoundary);
    }

    /// <summary>
    /// Gets the final, display-ready regex string for a given line, handling special display logic.
    /// </summary>
    /// <param name="line">The regex element to process.</param>
    /// <returns>The formatted regex string for display.</returns>
    string GetFinalRegexString(RegexElement line)
    {
        var regexText = line switch
        {
            SynonymSetHeader or SynonymTrailingSpacer => "",
            SynonymValueEnum synonymValueEnum => synonymValueEnum.CanonicalValue.ToString(),
            AlternateValueEnum alternateValueEnum => alternateValueEnum.DisplayOverrideName ?? alternateValueEnum.Regex,
            _ => line.Regex
        };

        if (line is AlternateValue)
            return regexText.Replace(" ", "[ ]");
        else
            return regexText;
    }

    /// <summary>
    /// Calculates the required column widths for aligning regex parts, comments, and comment boxes.
    /// </summary>
    /// <param name="lines">The list of all regex elements to be rendered.</param>
    /// <param name="alternateCounts">A dictionary of counts for enum alternates.</param>
    void CalculateColumnWidths(List<RegexElement> lines, IReadOnlyDictionary<AlternateValueEnum, int> alternateCounts)
    {
        int GetRenderedLineLength(RegexElement line)
        {
            int indentSpaces;
            if (line is AlternateValue)
            {
                var parentDepth = line.Enclosures.Count(e => e is not RootEnclosure) - 1;
                if (parentDepth < 0)
                    parentDepth = 0;

                const int alternatePrefixWidth = _spacesPerAlternateIndent + 2; // for "  | "
                indentSpaces = (parentDepth * _spacesPerIndent) + alternatePrefixWidth;
            }
            else
                indentSpaces = GetIndentDepth(line) * _spacesPerIndent;

            var length = indentSpaces;
            length += GetFinalRegexString(line).Length;
            return length;
        }

        var maxRegexLen = lines.Any() ? lines.Max(GetRenderedLineLength) : 0;
        _hashSeparatorColumn = maxRegexLen + _hashSeparatorPadding;
        _enumBoxMetrics = new Dictionary<string, EnumBoxLayoutMetrics>();
        var enumGroups = lines.OfType<AlternateValueEnum>().GroupBy(e => e.NamedPath);

        foreach (var group in enumGroups)
        {
            var maxValueLength = group.Max(alt => alt.Comment.Length);
            var maxCountLength = group.Max(alt => alternateCounts.TryGetValue(alt, out var count) ? count.ToString().Length : 0);
            _enumBoxMetrics[group.Key] = new EnumBoxLayoutMetrics(maxValueLength, maxCountLength);
        }

        var uniquePaths = lines
            .SelectMany(l => l.VisibleEnclosures.Select((e, i) => l.VisibleEnclosures.Take(i + 1)))
            .GroupBy(p => string.Join(",", p.Select(e => e.Ordinal)))
            .Select(g => g.First())
            .Where(p => p.Any())
            .ToList();

        var boxWidths = uniquePaths.ToDictionary(p => string.Join(",", p.Select(e => e.Ordinal)), p => 0);

        foreach (var line in lines.Where(l => l.VisibleEnclosures.Any()))
        {
            var pathKey = string.Join(",", line.VisibleEnclosures.Select(e => e.Ordinal));
            var requiredWidth = 0;
            if (!string.IsNullOrEmpty(line.Comment))
            {
                var textWidth = line switch
                {
                    AlternateValueEnum ave =>
                        _enumBoxMetrics.TryGetValue(ave.NamedPath, out var metrics)
                        ? metrics.MaxValueLength + (metrics.MaxCountLength > 0 ? 3 + metrics.MaxCountLength : 0) + 2
                        : 0,
                    BlankLine bl when bl.Comment.EndsWith("omitted") => bl.Comment.Length + 2,
                    AlternateValue or NamedGroupOpen or NamedGroupClose or GroupClose => line.Comment.Length + 2,
                    _ => _boxContentLeftPadding + line.Comment.Length
                };

                requiredWidth = line is EncloureBookend ? textWidth + 2 : textWidth + 4;
            }

            boxWidths[pathKey] = Math.Max(boxWidths[pathKey], requiredWidth);
        }

        var sortedPaths = uniquePaths.OrderByDescending(p => p.Count());

        foreach (var path in sortedPaths)
        {
            if (path.Count() <= 1)
                continue;

            var childPathKey = string.Join(",", path.Select(e => e.Ordinal));
            var parentPathKey = string.Join(",", path.Take(path.Count() - 1).Select(e => e.Ordinal));
            var childFootprint = boxWidths[childPathKey] + 4;
            boxWidths[parentPathKey] = Math.Max(boxWidths[parentPathKey], childFootprint);
        }

        var rootPaths = uniquePaths.Where(p => p.Count() == 1);
        _commentBoxLength = rootPaths.Any() ? rootPaths.Max(p => boxWidths[string.Join(",", p.Select(e => e.Ordinal))]) : 0;
    }

    /// <summary>
    /// Gets the indentation level for a given regex element based on its enclosure depth.
    /// </summary>
    /// <param name="line">The regex element.</param>
    /// <returns>The number of indent levels.</returns>
    int GetIndentDepth(RegexElement line)
    {
        var depth = line.Enclosures.Count(e => e is not RootEnclosure);

        if (depth == 0)
            return 0;

        return line is EncloureBookend ? depth - 1 : depth;
    }

    /// <summary>
    /// Determines the primary color for the regex content of a line.
    /// </summary>
    /// <param name="line">The regex element.</param>
    /// <returns>A hex color string.</returns>
    string GetPrimaryContentColorForLine(RegexElement line)
    {
        if (!line.VisibleEnclosures.Any())
        {
            return line switch
            {
                TextLine => _colors.UnenclosedTextLineCommentColor,
                SpaceLine => _colors.UnenclosedSpaceLineCommentColor,
                BoundaryBase => _colors.BoundaryCommentColor,
                _ => _colors.DefaultFallbackColor
            };
        }

        var currentEnclosure = line.VisibleEnclosures.Last();
        var palette = currentEnclosure.Palette;

        switch (line)
        {
            case NamedGroupOpen or NamedGroupClose: return _colors.NamedGroupBookendCommentColor(palette);
            case GroupOpen: return DefaultWhite;
            case GroupClose: return _colors.GroupCloseQuantifierColor;
            case AlternateValue: return _colors.AlternateValueCommentColor(palette);
            case TextLine or SpaceLine or GroupAlternativePipe:
                var nearestNamedEnclosure = line.VisibleEnclosures.LastOrDefault(e => e is NamedEnclosure) as NamedEnclosure;
                return nearestNamedEnclosure != null ? _colors.EnclosedTextColor(nearestNamedEnclosure.Palette) : DefaultWhite;
            default: return DefaultWhite;
        }
    }

    /// <summary>
    /// Generates the list of colored and styled spans for the comment part of a line.
    /// </summary>
    /// <param name="line">The regex element for which to generate comment spans.</param>
    /// <param name="alternateCounts">A dictionary of counts for enum alternates.</param>
    /// <returns>A list of comment line spans.</returns>
    List<RegexCommentedLineSpan> GenerateCommentSpans(RegexElement line, IReadOnlyDictionary<AlternateValueEnum, int> alternateCounts)
    {
        var spans = new List<RegexCommentedLineSpan>();
        var lowlight = _treatments.CommentLowlightTreatment;

        void AddSpanForEnclosurePath(string text, HexPalette palette, SpanHighlightTreatment highlight, IEnumerable<Enclosure> scope)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var rootName = line.Enclosures.OfType<RootEnclosure>().FirstOrDefault()?.RootTypeName ?? "";
            var namedPath = string.Join('.', scope.OfType<NamedEnclosure>().Select(x => x.Name));
            var fullPath = string.IsNullOrEmpty(namedPath) ? rootName : $"{rootName}.{namedPath}";
            var relativePath = RegexCommentedLine.GetRelativePath(fullPath);
            spans.Add(new(text, palette, relativePath, relativePath == null ? SpanHighlightTreatment.None : highlight, lowlight));
        }

        void AddSpanForCurrentLine(string text, HexPalette palette, bool isTextSpan)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var path = line.NamedPath;
            if (line is SynonymValueEnum syn)
                path = $"{path}.{syn.CanonicalParent.CanonicalValue}.{syn.CanonicalValue}";
            else if (line is AlternateValueEnum ave)
                path = $"{path}.{ave.CanonicalValue}";
            else if (line is AlternateValue av)
                path = $"{path}.{av.CanonicalValue}";

            var relativePath = RegexCommentedLine.GetRelativePath(path);
            var highlight = _treatments.GetCommentHighlightTreatment(line, isTextSpan);
            spans.Add(new(text, palette, relativePath, relativePath == null ? SpanHighlightTreatment.None : highlight, lowlight));
        }

        var defaultWhitePalette = DeterministicPalette.GetStaticPalette(new HexColor(DefaultWhite));
        var visibleEnclosures = line.VisibleEnclosures.ToArray();

        if (visibleEnclosures.Length == 0)
        {
            var color = GetPrimaryContentColorForLine(line);
            spans.Add(new(line.Comment ?? "", DeterministicPalette.GetStaticPalette(new HexColor(color)), null, SpanHighlightTreatment.None, lowlight));
            return spans;
        }

        var parentEnclosures = visibleEnclosures.Take(visibleEnclosures.Length - 1).ToList();
        var currentEnclosure = visibleEnclosures.Last();
        var chars = BoxChars.Get(currentEnclosure.Treatment);
        var palette = currentEnclosure.Palette;
        var borderPalette = DeterministicPalette.GetStaticPalette(new HexColor(_colors.GetBorderColor(currentEnclosure.Treatment, palette)));
        var borderHighlight = _treatments.GetCommentHighlightTreatment(line, false);
        var currentPathParts = new List<Enclosure>();

        foreach (var parent in parentEnclosures)
        {
            currentPathParts.Add(parent);
            var wall = BoxChars.Get(parent.Treatment).Wall;
            var parentBorderPalette = DeterministicPalette.GetStaticPalette(new HexColor(_colors.GetBorderColor(parent.Treatment, parent.Palette)));
            AddSpanForEnclosurePath(wall.ToString(), parentBorderPalette, borderHighlight, currentPathParts);
            AddSpanForEnclosurePath(" ", defaultWhitePalette, borderHighlight, currentPathParts);
        }

        var currentLevelWidth = _commentBoxLength - (parentEnclosures.Count * 4);

        if (line is EncloureBookend)
        {
            var availableWidth = currentLevelWidth - 2;
            switch (line)
            {
                case NamedGroupOpen ngo:
                    var openComment = $" {ngo.Comment} ";
                    AddSpanForCurrentLine(chars.TopLeft.ToString(), borderPalette, false);
                    AddSpanForCurrentLine(openComment, DeterministicPalette.GetStaticPalette(new HexColor(_colors.NamedGroupBookendCommentColor(palette))), true);
                    AddSpanForCurrentLine(new string(chars.Top, Math.Max(0, availableWidth - openComment.Length)), borderPalette, false);
                    AddSpanForCurrentLine(chars.TopRight.ToString(), borderPalette, false);
                    break;
                case GroupOpen:
                    AddSpanForCurrentLine(chars.TopLeft.ToString() + new string(chars.Top, availableWidth) + chars.TopRight.ToString(), borderPalette, false);
                    break;
                case NamedGroupClose ngc:
                    var closeComment = $" {ngc.Comment} ";
                    AddSpanForCurrentLine(chars.BottomLeft.ToString(), borderPalette, false);
                    AddSpanForCurrentLine(new string(chars.Bottom, Math.Max(0, availableWidth - closeComment.Length)), borderPalette, false);
                    AddSpanForCurrentLine(closeComment, DeterministicPalette.GetStaticPalette(new HexColor(_colors.NamedGroupBookendCommentColor(palette))), true);
                    AddSpanForCurrentLine(chars.BottomRight.ToString(), borderPalette, false);
                    break;
                case GroupClose gc:
                    var quantComment = gc.Comment != null ? $" {gc.Comment} " : "";
                    AddSpanForCurrentLine(chars.BottomLeft.ToString(), borderPalette, false);
                    AddSpanForCurrentLine(new string(chars.Bottom, Math.Max(0, availableWidth - quantComment.Length)), borderPalette, false);
                    AddSpanForCurrentLine(quantComment, DeterministicPalette.GetStaticPalette(new HexColor(_colors.GroupCloseQuantifierColor)), true);
                    AddSpanForCurrentLine(chars.BottomRight.ToString(), borderPalette, false);
                    break;
            }
        }
        else
        {
            var innerWidth = currentLevelWidth - 4;
            AddSpanForEnclosurePath(chars.Wall + " ", borderPalette, borderHighlight, visibleEnclosures);

            switch (line)
            {
                case SynonymTrailingSpacer:
                    AddSpanForCurrentLine(new string('-', innerWidth), palette, true);
                    break;
                case AlternateValueEnum ave when _enumBoxMetrics.TryGetValue(ave.NamedPath, out var metrics):
                    {
                        string lineText;
                        string fullContent;
                        HexPalette altPalette;

                        if (alternateCounts.TryGetValue(ave, out var count))
                        {
                            altPalette = DeterministicPalette.GetStaticPalette(new HexColor(ave is SynonymValueEnum ? _colors.SynonymValueCommentColor(palette) : _colors.AlternateValueCommentColor(palette)));
                            lineText = $"{ave.Comment.PadLeft(metrics.MaxValueLength)} : {count}";
                            var totalPad = Math.Max(0, innerWidth - (metrics.MaxValueLength + 3 + metrics.MaxCountLength));
                            fullContent = $"{new string(' ', totalPad / 2)}{lineText}{new string(' ', metrics.MaxValueLength + 3 + metrics.MaxCountLength - lineText.Length)}{new string(' ', totalPad - (totalPad / 2))}";
                        }
                        else
                        {
                            altPalette = DeterministicPalette.GetStaticPalette(new HexColor(_colors.AlternateValueCommentColor(palette)));
                            lineText = ave.Comment.PadLeft(metrics.MaxValueLength);
                            var totalPad = Math.Max(0, innerWidth - metrics.MaxValueLength);
                            fullContent = $"{new string(' ', totalPad / 2)}{lineText}{new string(' ', totalPad - (totalPad / 2))}";
                        }
                        AddSpanForCurrentLine(fullContent, altPalette, true);
                        break;
                    }
                case BlankLine bl when !string.IsNullOrEmpty(bl.Comment) && bl.Comment.EndsWith("omitted"):
                    var omitPad = Math.Max(0, innerWidth - bl.Comment.Length);
                    AddSpanForCurrentLine($"{new string(' ', omitPad / 2)}{bl.Comment}{new string(' ', omitPad - (omitPad / 2))}", DeterministicPalette.GetStaticPalette(new HexColor(_colors.OmittedEnumCountColor)), true);
                    break;
                case AlternateValue av:
                    var altCommentText = $" {av.Comment} ";
                    var genericTotalPad = Math.Max(0, innerWidth - altCommentText.Length);
                    AddSpanForCurrentLine($"{new string(' ', genericTotalPad / 2)}{altCommentText}{new string(' ', genericTotalPad - (genericTotalPad / 2))}", DeterministicPalette.GetStaticPalette(new HexColor(_colors.AlternateValueCommentColor(palette))), true);
                    break;
                default:
                    var nearestNamed = visibleEnclosures.LastOrDefault(e => e is NamedEnclosure) as NamedEnclosure;
                    var contentPalette = nearestNamed != null ? DeterministicPalette.GetStaticPalette(new HexColor(_colors.EnclosedTextColor(nearestNamed.Palette))) : defaultWhitePalette;
                    var content = string.IsNullOrEmpty(line.Comment) ? new string(' ', innerWidth) : (new string(' ', _boxContentLeftPadding) + line.Comment).PadRight(innerWidth);
                    AddSpanForCurrentLine(content, contentPalette, true);
                    break;
            }

            AddSpanForEnclosurePath(" " + chars.Wall, borderPalette, borderHighlight, visibleEnclosures);
        }

        var parentsReversed = parentEnclosures.AsEnumerable().Reverse().ToList();
        for (int i = 0; i < parentsReversed.Count(); i++)
        {
            var parent = parentsReversed[i];
            var pathForParent = visibleEnclosures.Take(parentEnclosures.Count - i).ToList();
            var wall = BoxChars.Get(parent.Treatment).Wall;
            var parentBorderPalette = DeterministicPalette.GetStaticPalette(new HexColor(_colors.GetBorderColor(parent.Treatment, parent.Palette)));
            AddSpanForEnclosurePath(" ", defaultWhitePalette, borderHighlight, pathForParent);
            AddSpanForEnclosurePath(wall.ToString(), parentBorderPalette, borderHighlight, pathForParent);
        }

        return spans;
    }
}