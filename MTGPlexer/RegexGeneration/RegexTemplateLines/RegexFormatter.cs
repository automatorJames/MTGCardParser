namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

/// <summary>
/// Handles the formatting and presentation of a sequence of regular expression elements.
/// </summary>
public class RegexFormatter
{
    IReadOnlyList<RegexElement> _regexElements;
    List<PropPathSynonymSetContainer> _synonymData;

    readonly FormattedRegexColoringRules _colors = new();
    readonly FormattedRegexTreatmentRules _treatments = new();

    int _hashSeparatorColumn;
    int _commentBoxLength;
    Dictionary<string, EnumBoxLayoutMetrics> _enumBoxMetrics = [];
    Dictionary<Enclosure, HexPalette> _enclosurePalettes = [];

    const int HashSeparatorPadding = 4;
    const int BoxContentLeftPadding = 1;
    const int SpacesPerIndent = 4;
    const int SpacesPerAlternateIndent = 2;

    public RegexFormatter(IReadOnlyList<RegexElement> regexElements, List<PropPathSynonymSetContainer> synonymData)
    {
        _regexElements = regexElements;
        _synonymData = synonymData;

        var allNamedEnclosures = _regexElements
            .SelectMany(x => x.Enclosures)
            .OfType<NamedEnclosure>()
            .Distinct()
            .ToList();

        var positionalPalettes = DeterministicPalette.GetPositionalPaletteSet(allNamedEnclosures.Count);

        // Set named enclosure palettes
        for (int i = 0; i < allNamedEnclosures.Count; i++)
            _enclosurePalettes[allNamedEnclosures[i]] = positionalPalettes[i];

        // Let elements whose immediate parents are non-named inherit their nearst named enclosure's palette
        foreach (var element in _regexElements.Where(x => x.ParentEnclosure != null && !_enclosurePalettes.ContainsKey(x.ParentEnclosure)))
            if (!_enclosurePalettes.ContainsKey(element.ParentEnclosure) && element.ParentNamedEnclosure is NamedEnclosure closestNamedParent)
                _enclosurePalettes[element.ParentEnclosure] = _enclosurePalettes[closestNamedParent];
    }

    /// <summary>
    /// Formats a list of RegexElement objects into a human-readable, commented output.
    /// </summary>
    public List<RegexFormattedLine> Format()
    {
        if (_regexElements == null || !_regexElements.Any()) return [];

        // 1. Expansion: Flatten containers and inject synonym variants
        var expandedElements = ExpandAlternateElements(_regexElements, _synonymData ?? [], out var alternateCounts);

        // 2. Vertical Spacing: Add blank lines to separate logical blocks
        var spacedElements = InsertVerticalBreathingRoom(expandedElements);

        // 3. Layout: Calculate column widths for alignment
        CalculateLayoutMetrics(spacedElements, alternateCounts);

        // 4. Rendering: Convert logical elements into formatted spans
        var renderedLines = spacedElements.Select(line => CreateCommentedLine(line, alternateCounts)).ToList();

        // 5. Post-Processing: Fix the alignment and prefixes for '|' alternate lines
        return FinalizeAlternatePrefixes(renderedLines);
    }

    #region Phase 1: Expansion

    List<RegexElement> ExpandAlternateElements(IReadOnlyList<RegexElement> elements, List<PropPathSynonymSetContainer> synonymData, out Dictionary<AlternateValueEnum, int> alternateCounts)
    {
        alternateCounts = new Dictionary<AlternateValueEnum, int>();
        var synonymLookup = synonymData.ToDictionary(d => d.ParentPath.PropPath);
        var result = new List<RegexElement>();

        foreach (var element in elements)
        {
            if (element is AlternateValueEnumContainer enumContainer)
                ExpandAlternateEnumContainer(enumContainer, synonymLookup, result, alternateCounts);
            else if (element is AlternateValueContainer container)
                result.AddRange(container.AlternateValues);
            else
                result.Add(element);
        }

        return result;
    }

    void ExpandAlternateEnumContainer(AlternateValueEnumContainer container, Dictionary<string, PropPathSynonymSetContainer> lookup, List<RegexElement> output, Dictionary<AlternateValueEnum, int> counts)
    {
        if (!lookup.TryGetValue(container.NamedPath, out var wrapper))
        {
            output.Add(new BlankLine(container.Enclosures) { Comment = $"All {container.AlternateValueEnums.Count} omitted" });
            return;
        }

        SynonymTrailingSpacer spacerBuffer = null;

        foreach (var synonymSet in wrapper.SynonymSets.Values)
        {
            if (spacerBuffer != null)
            {
                output.Add(spacerBuffer);
                spacerBuffer = null;
            }

            var enumElement = container.AlternateValueEnums.Single(e => e.CanonicalValue.Equals(synonymSet.CanonicalValue));

            if (synonymSet.SynonymCounts.Count > 1)
            {
                // Multi-synonym set: Add header and individual variants
                var header = new SynonymSetHeader(enumElement);
                counts[header] = synonymSet.TotalCount;
                output.Add(header);

                foreach (var synonym in synonymSet.SynonymCounts)
                {
                    var synonymElement = new SynonymValueEnum(enumElement, synonymSet.TotalCount, synonym.Key);
                    counts[synonymElement] = synonym.Value;
                    output.Add(synonymElement);
                }
                spacerBuffer = new SynonymTrailingSpacer(enumElement);
            }
            else
            {
                // Single value: Just use the name
                enumElement.DisplayOverrideName = synonymSet.SynonymCounts.First().Key;
                output.Add(enumElement);
                counts[enumElement] = synonymSet.TotalCount;
            }
        }

        if (wrapper.UnrepresentedAlternateCount > 0)
            output.Add(new BlankLine(container.Enclosures) { Comment = $"{wrapper.UnrepresentedAlternateCount} omitted" });
    }

    #endregion

    #region Phase 2: Vertical Spacing

    List<RegexElement> InsertVerticalBreathingRoom(List<RegexElement> elements)
    {
        if (elements.Count == 0) 
            return elements;

        var result = new List<RegexElement> { elements[0] };
        for (var i = 1; i < elements.Count; i++)
        {
            var prev = elements[i - 1];
            var curr = elements[i];

            bool pathChanged = curr.UniquePath != prev.UniquePath;
            bool spaceAbutsLiteral = (prev is TextLine or AtomElement && curr is SpaceLine) || (prev is SpaceLine && curr is TextLine or AtomElement);

            if (pathChanged || spaceAbutsLiteral)
            {
                int prevType = GetEnclosureEventType(prev);
                int currType = GetEnclosureEventType(curr);

                // Add a blank line if transitioning between structure (Open/Close) and content,
                // or between two different structural events.
                if (prevType != currType || prevType == 0 || spaceAbutsLiteral)
                {
                    int commonDepth = CalculateCommonDepth(prev, curr);
                    result.Add(new BlankLine(prev.Enclosures.Take(commonDepth).ToArray()));
                }
            }
            result.Add(curr);
        }
        return result;
    }

    int GetEnclosureEventType(RegexElement line) => line switch
    {
        AnonymousGroupOpen or NamedGroupOpen => 1,
        AnonymousGroupClose or NamedGroupClose => 2,
        _ => 0
    };

    int CalculateCommonDepth(RegexElement a, RegexElement b)
    {
        int depth = 0;

        while (depth < a.Enclosures.Length && depth < b.Enclosures.Length && a.Enclosures[depth].Ordinal == b.Enclosures[depth].Ordinal)
            depth++;

        return depth;
    }

    #endregion

    #region Phase 3: Layout Measurement

    void CalculateLayoutMetrics(List<RegexElement> lines, Dictionary<AlternateValueEnum, int> counts)
    {
        // 1. Determine the widest regex string to set the '#' column
        int maxRegexWidth = lines.Any() ? lines.Max(GetLineRenderedLength) : 0;
        _hashSeparatorColumn = maxRegexWidth + HashSeparatorPadding;

        // 2. Calculate Enum internal box metrics (column for value names vs column for counts)
        _enumBoxMetrics = lines.OfType<AlternateValueEnum>()
            .GroupBy(e => e.NamedPath)
            .ToDictionary(
                g => g.Key,
                g => new EnumBoxLayoutMetrics(
                    g.Max(alt => alt.Comment.Length),
                    g.Max(alt => counts.TryGetValue(alt, out var c) ? c.ToString().Length : 0)
                )
            );

        // 3. Calculate recursive box widths (inner boxes force parent boxes to be wider)
        MeasureCommentBoxes(lines);
    }

    void MeasureCommentBoxes(List<RegexElement> lines)
    {
        var uniquePaths = lines
            .SelectMany(l => l.VisibleEnclosures.Select((_, i) => l.VisibleEnclosures.Take(i + 1)))
            .GroupBy(p => string.Join(",", p.Select(e => e.Ordinal)))
            .Select(g => g.First())
            .Where(p => p.Any())
            .ToList();

        var boxWidths = uniquePaths.ToDictionary(p => GetPathKey(p), _ => 0);

        // Initial pass: width based on content of the lines themselves
        foreach (var line in lines.Where(l => l.VisibleEnclosures.Any()))
        {
            var key = GetPathKey(line.VisibleEnclosures);
            int contentWidth = CalculateInternalCommentWidth(line);
            boxWidths[key] = Math.Max(boxWidths[key], contentWidth);
        }

        // Propagate widths: parent boxes must be wide enough to contain child boxes + padding
        foreach (var path in uniquePaths.OrderByDescending(p => p.Count()))
        {
            if (path.Count() <= 1) continue;
            var parentKey = GetPathKey(path.Take(path.Count() - 1));
            boxWidths[parentKey] = Math.Max(boxWidths[parentKey], boxWidths[GetPathKey(path)] + 4);
        }

        var rootPaths = uniquePaths.Where(p => p.Count() == 1);
        _commentBoxLength = rootPaths.Any() ? rootPaths.Max(p => boxWidths[GetPathKey(p)]) : 0;
    }

    string GetPathKey(IEnumerable<Enclosure> enclosures) => string.Join(",", enclosures.Select(e => e.Ordinal));

    int CalculateInternalCommentWidth(RegexElement line)
    {
        if (string.IsNullOrEmpty(line.Comment)) return 0;

        int textWidth = line switch
        {
            AlternateValueEnum ave => _enumBoxMetrics.TryGetValue(ave.NamedPath, out var m)
                ? m.MaxValueLength + (m.MaxCountLength > 0 ? 3 + m.MaxCountLength : 0) + 2 : 0,
            BlankLine bl when bl.Comment.EndsWith("omitted") => bl.Comment.Length + 2,
            AlternateValue or NamedGroupOpen or NamedGroupClose or AnonymousGroupClose => line.Comment.Length + 2,
            _ => BoxContentLeftPadding + line.Comment.Length
        };

        return line is EnclosureBookend ? textWidth + 2 : textWidth + 4;
    }

    #endregion

    #region Phase 4: Line Rendering

    RegexFormattedLine CreateCommentedLine(RegexElement line, IReadOnlyDictionary<AlternateValueEnum, int> alternateCounts)
    {
        var regexText = GetFinalRegexString(line);
        int indent = (line is AlternateValue ? GetAlternateIndent(line) : GetIndentDepth(line)) * SpacesPerIndent;

        var paddedRegex = (new string(' ', indent) + regexText).PadRight(_hashSeparatorColumn);
        var spans = new List<RegexCommentedLineSpan>();

        // 1. Regex Span
        var primaryColor = GetPrimaryContentColorForLine(line);
        var highlight = _treatments.GetRegexHighlightTreatment(line);
        spans.Add(new(paddedRegex, DeterministicPalette.GetStaticPalette(new HexColor(primaryColor)),
            RegexFormattedLine.GetRelativePath(GetPathForSpan(line)), highlight, _treatments.CommentLowlightTreatment));

        // 2. Hash Separator
        var commentPrefix = $"#{new string(' ', HashSeparatorPadding)}";
        spans.Add(new(commentPrefix, DeterministicPalette.GetStaticPalette(new HexColor(_colors.HashSeparatorColor)),
            null, SpanHighlightTreatment.None, SpanLowlightTreatment.None));

        // 3. Comment Content Spans
        var commentSpans = GenerateCommentSpans(line, alternateCounts);
        spans.AddRange(commentSpans);

        var fullComment = commentPrefix + string.Join("", commentSpans.Select(s => s.SpanText));

        if (line is AlternateValue av && line is not SynonymSetHeader && line is not SynonymTrailingSpacer)
            return new RegexCommentedAlternateLine(paddedRegex, fullComment, line.NamedPath, spans, av);

        return new RegexFormattedLine(paddedRegex, fullComment, line.NamedPath, spans);
    }

    List<RegexCommentedLineSpan> GenerateCommentSpans(RegexElement line, IReadOnlyDictionary<AlternateValueEnum, int> alternateCounts)
    {
        var spans = new List<RegexCommentedLineSpan>();
        var visibleEnclosures = line.VisibleEnclosures.ToArray();

        // Handle unenclosed lines (Boundaries, etc.)
        if (visibleEnclosures.Length == 0)
        {
            var color = GetPrimaryContentColorForLine(line);

            RegexCommentedLineSpan span = new(
                line.Comment ?? "", DeterministicPalette.GetStaticPalette(new HexColor(color)),
                null,
                SpanHighlightTreatment.None,
                _treatments.CommentLowlightTreatment);

            spans.Add(span);

            return spans;
        }

        var parentEnclosures = visibleEnclosures.Take(visibleEnclosures.Length - 1).ToList();
        var current = visibleEnclosures.Last();
        var chars = BoxChars.Get(current.Treatment);
        var borderPalette = DeterministicPalette.GetStaticPalette(new HexColor(_colors.GetBorderColor(current.Treatment, _enclosurePalettes[current])));

        // Draw Left-side parent walls
        AddParentBorders(line, parentEnclosures, spans, isClosing: false);

        int currentLevelWidth = _commentBoxLength - (parentEnclosures.Count * 4);

        if (line is EnclosureBookend bookend)
        {
            RenderBookendComment(bookend, spans, chars, borderPalette, currentLevelWidth - 2);
        }
        else
        {
            // Internal Content
            AddEnclosurePathSpan(line, spans, chars.Wall + " ", borderPalette, visibleEnclosures);
            RenderInternalComment(line, spans, currentLevelWidth - 4, alternateCounts, _enclosurePalettes[current]);
            AddEnclosurePathSpan(line, spans, " " + chars.Wall, borderPalette, visibleEnclosures);
        }

        // Draw Right-side parent walls
        AddParentBorders(line, parentEnclosures, spans, isClosing: true);

        return spans;
    }

    #endregion

    #region Helpers & Sub-Renderers

    void AddParentBorders(RegexElement line, List<Enclosure> parents, List<RegexCommentedLineSpan> spans, bool isClosing)
    {
        var list = isClosing ? parents.AsEnumerable().Reverse().ToList() : parents;
        var white = DeterministicPalette.GetStaticPalette(new HexColor(FormattedRegexColoringRules.White));

        for (int i = 0; i < list.Count; i++)
        {
            var parent = list[i];
            var scope = isClosing ? line.VisibleEnclosures.Take(parents.Count - i).ToList() : line.VisibleEnclosures.Take(i + 1).ToList();
            var wall = BoxChars.Get(parent.Treatment).Wall.ToString();
            var palette = DeterministicPalette.GetStaticPalette(new HexColor(_colors.GetBorderColor(parent.Treatment, _enclosurePalettes[parent])));

            if (isClosing) 
                AddEnclosurePathSpan(line, spans, " ", white, scope);

            AddEnclosurePathSpan(line, spans, wall, palette, scope);

            if (!isClosing) 
                AddEnclosurePathSpan(line, spans, " ", white, scope);
        }
    }

    void RenderBookendComment(EnclosureBookend line, List<RegexCommentedLineSpan> spans, BoxCharSet chars, HexPalette borderPal, int width)
    {
        string comment = line.Comment != null ? $" {line.Comment} " : "";
        string color = line switch
        {
            NamedGroupOpen or NamedGroupClose => _colors.NamedGroupBookendCommentColor(_enclosurePalettes[line.Enclosures.Last()]),
            AnonymousGroupClose => _colors.AnonymousGroupBookendColor,
            _ => FormattedRegexColoringRules.White
        };

        char left = line is NamedGroupOpen or AnonymousGroupOpen ? chars.TopLeft : chars.BottomLeft;
        char right = line is NamedGroupOpen or AnonymousGroupOpen ? chars.TopRight : chars.BottomRight;
        char fill = line is NamedGroupOpen or AnonymousGroupOpen ? chars.Top : chars.Bottom;

        AddCurrentLineSpan(line, spans, left.ToString(), borderPal, false);

        if (line is NamedGroupOpen or AnonymousGroupClose) // Comments are left-aligned or right-aligned based on type
        {
            if (line is NamedGroupOpen) 
                AddCurrentLineSpan(line, spans, comment, DeterministicPalette.GetStaticPalette(new HexColor(color)), true);

            AddCurrentLineSpan(line, spans, new string(fill, Math.Max(0, width - comment.Length)), borderPal, false);

            if (line is AnonymousGroupClose) 
                AddCurrentLineSpan(line, spans, comment, DeterministicPalette.GetStaticPalette(new HexColor(color)), true);
        }
        else // NamedGroupClose has comment at the end
        {
            AddCurrentLineSpan(line, spans, new string(fill, Math.Max(0, width - comment.Length)), borderPal, false);
            AddCurrentLineSpan(line, spans, comment, DeterministicPalette.GetStaticPalette(new HexColor(color)), true);
        }

        AddCurrentLineSpan(line, spans, right.ToString(), borderPal, false);
    }

    void RenderInternalComment(RegexElement line, List<RegexCommentedLineSpan> spans, int width, IReadOnlyDictionary<AlternateValueEnum, int> counts, HexPalette palette)
    {
        var white = DeterministicPalette.GetStaticPalette(new HexColor(FormattedRegexColoringRules.White));

        switch (line)
        {
            case SynonymTrailingSpacer:
                AddCurrentLineSpan(line, spans, new string('-', width), palette, true);
                break;

            case AlternateValueEnum ave when _enumBoxMetrics.TryGetValue(ave.NamedPath, out var m):
                bool hasCount = counts.TryGetValue(ave, out var count);
                var color = ave is SynonymValueEnum ? _colors.SynonymValueCommentColor(palette) : _colors.AlternateValueCommentColor(palette);
                string text = hasCount ? $"{ave.Comment.PadLeft(m.MaxValueLength)} : {count}" : ave.Comment.PadLeft(m.MaxValueLength);
                int pad = Math.Max(0, width - text.Length);
                string full = $"{new string(' ', pad / 2)}{text}{new string(' ', pad - (pad / 2))}";
                AddCurrentLineSpan(line, spans, full, DeterministicPalette.GetStaticPalette(new HexColor(color)), true);
                break;

            case BlankLine bl when bl.Comment?.EndsWith("omitted") == true:
                int oPad = Math.Max(0, width - bl.Comment.Length);
                string oFull = $"{new string(' ', oPad / 2)}{bl.Comment}{new string(' ', oPad - (oPad / 2))}";
                AddCurrentLineSpan(line, spans, oFull, DeterministicPalette.GetStaticPalette(new HexColor(_colors.OmittedEnumCountColor)), true);
                break;

            default:
                var content = string.IsNullOrEmpty(line.Comment) ? new string(' ', width) : (new string(' ', BoxContentLeftPadding) + line.Comment).PadRight(width);
                var nearNamed = line.VisibleEnclosures.LastOrDefault(e => e is NamedEnclosure) as NamedEnclosure;
                var cPal = nearNamed != null ? DeterministicPalette.GetStaticPalette(new HexColor(_colors.EnclosedTextColor(_enclosurePalettes[nearNamed]))) : white;
                AddCurrentLineSpan(line, spans, content, cPal, true);
                break;
        }
    }

    string GetPathForSpan(RegexElement line) => line switch
    {
        SynonymValueEnum s => $"{line.NamedPath}.{s.CanonicalValue}",
        AlternateValueEnum a => $"{line.NamedPath}.{a.CanonicalValue}",
        AlternateValue v => $"{line.NamedPath}.{v.CanonicalValue}",
        _ => line.NamedPath
    };

    void AddEnclosurePathSpan(RegexElement line, List<RegexCommentedLineSpan> spans, string text, HexPalette pal, IEnumerable<Enclosure> scope)
    {
        var root = line.Enclosures.OfType<RootEnclosure>().FirstOrDefault()?.RootTypeName ?? "";
        var path = string.Join('.', scope.OfType<NamedEnclosure>().Select(x => x.Name));
        var full = string.IsNullOrEmpty(path) ? root : $"{root}.{path}";
        var rel = RegexFormattedLine.GetRelativePath(full);
        spans.Add(new(text, pal, rel, rel == null ? SpanHighlightTreatment.None : _treatments.GetCommentHighlightTreatment(line, false), _treatments.CommentLowlightTreatment));
    }

    void AddCurrentLineSpan(RegexElement line, List<RegexCommentedLineSpan> spans, string text, HexPalette pal, bool isText)
    {
        var rel = RegexFormattedLine.GetRelativePath(GetPathForSpan(line));
        var high = _treatments.GetCommentHighlightTreatment(line, isText);
        spans.Add(new(text, pal, rel, rel == null ? SpanHighlightTreatment.None : high, _treatments.CommentLowlightTreatment));
    }

    List<RegexFormattedLine> FinalizeAlternatePrefixes(List<RegexFormattedLine> lines)
    {
        var result = new List<RegexFormattedLine>();
        string activePath = null;
        bool first = true;

        foreach (var line in lines)
        {
            if (line is RegexCommentedAlternateLine alt)
            {
                if (alt.EnclosurePath != activePath) { activePath = alt.EnclosurePath; first = true; }

                var trimmed = alt.Regex.Trim();
                int startCol = alt.Regex.Length - alt.Regex.TrimStart().Length;
                string newRegex;

                if (first)
                    newRegex = new string(' ', startCol) + trimmed;
                else
                {
                    var prefix = $"{new string(' ', SpacesPerAlternateIndent)}| ";
                    newRegex = new string(' ', Math.Max(0, startCol - prefix.Length)) + prefix + trimmed;
                }

                var padded = newRegex.PadRight(_hashSeparatorColumn);
                var newSpans = alt.Spans.ToList();
                newSpans[0] = newSpans[0] with { SpanText = padded };
                result.Add(alt with { Regex = padded, FormattedText = padded + alt.Comment, Spans = newSpans });
                first = false;
            }
            else
            {
                activePath = null;
                result.Add(line);
            }
        }

        return result;
    }

    string GetFinalRegexString(RegexElement line)
    {
        var text = line switch
        {
            SynonymSetHeader or SynonymTrailingSpacer => "",
            SynonymValueEnum syn => syn.CanonicalValue.ToString(),
            AlternateValueEnum ave => ave.DisplayOverrideName ?? ave.Regex,
            _ => line.Regex
        };

        return line is AlternateValue ? text.Replace(" ", "[ ]") : text;
    }

    int GetLineRenderedLength(RegexElement line)
    {
        int indent = (line is AlternateValue ? GetAlternateIndent(line) : GetIndentDepth(line)) * SpacesPerIndent;
        return indent + GetFinalRegexString(line).Length;
    }

    int GetIndentDepth(RegexElement line)
    {
        int d = line.Enclosures.Count(e => e is not RootEnclosure);
        return (d > 0 && line is EnclosureBookend) ? d - 1 : d;
    }

    int GetAlternateIndent(RegexElement line)
    {
        int d = line.Enclosures.Count(e => e is not RootEnclosure) - 1;
        return (d < 0 ? 0 : d) + 1; // +1 to account for the "| " prefix logic
    }

    string GetPrimaryContentColorForLine(RegexElement line)
    {
        if (!line.VisibleEnclosures.Any()) 
            return line switch
            {
                TextLine => _colors.UnenclosedTextLineCommentColor,
                SpaceLine => _colors.UnenclosedSpaceLineCommentColor,
                BoundaryBase => _colors.BoundaryCommentColor,
                _ => _colors.DefaultFallbackColor
            };

        var palette = _enclosurePalettes[line.VisibleEnclosures.Last()];

        return line switch
        {
            NamedGroupOpen or NamedGroupClose => _colors.NamedGroupBookendCommentColor(palette),
            AnonymousGroupOpen or AnonymousGroupClose => _colors.AnonymousGroupBookendColor,
            AlternateValue => _colors.AlternateValueCommentColor(palette),
            _ => (line.VisibleEnclosures.LastOrDefault(e => e is NamedEnclosure) is NamedEnclosure x)
                ? _colors.EnclosedTextColor(_enclosurePalettes[x]) : FormattedRegexColoringRules.White
        };
    }

    #endregion
}