using System.Diagnostics;

namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public class RegexBuilder
{
    List<RegexElement> _regexElements = [];
    int _nextEnclosureOrdinal;
    Stack<Enclosure> _enclosureStack = [];
    Dictionary<Enclosure, int> _enclosureTerminalPropCount = [];
    BoundaryOption _boundaryOption;

    Dictionary<Enclosure, SpaceDisposition> _spaceIsRequiredBeforeNextElementAtLevel;

    Enclosure[] _orderedEnclosureStack =>
        _enclosureStack
            .Reverse()
            .ToArray();

    // Fields to support formatting, adapted from FormattedRegex
    private FormattedRegexColoringRules _colors;
    private FormattedRegexTreatmentRules _treatments;
    private int _hashSeparatorColumn;
    private int _commentBoxLength;
    private const string DefaultWhite = "#FFFFFF";
    private const int _hashSeparatorPadding = 6;
    private const int _boxContentLeftPadding = 1;
    private const int _spacesPerIndent = 4;

    // New field to hold layout metrics for enum comment boxes
    private record EnumBoxLayoutMetrics(int MaxValueLength, int MaxCountLength);
    private Dictionary<string, EnumBoxLayoutMetrics> _enumBoxMetrics;


    public RegexBuilder(Type topLevelType, bool neverAddSpacesAtTopLevel = false)
    {
        // an invisible top level enclosure;
        RootEnclosure rootEnclosure = new(topLevelType.Name);

        // always track the root enclosure (makes space disposition tracking cleaner)
        _enclosureStack.Push(rootEnclosure);

        _boundaryOption = topLevelType.GetCustomAttribute<RegexBoundaryOptionAtrribute>()?.Option ?? BoundaryOption.WholeWord;

        var topLevelSpaceDiposition = (topLevelType.IsDefined(typeof(NoSpacesAttribute)) || neverAddSpacesAtTopLevel)
            ? SpaceDisposition.NeverAddSpaceLocal
            : SpaceDisposition.DontAddSpaceBeforeNextItem;

        _spaceIsRequiredBeforeNextElementAtLevel = new Dictionary<Enclosure, SpaceDisposition> { [rootEnclosure] = topLevelSpaceDiposition };
    }

    public void OpenGroup(RegexPropInfo captureGroup = null, SpaceDisposition? spaceDisposition = null, string nameOverride = null)
    {
        AddPrecedingSpaceIfApplicable();
        Enclosure enclosure = null;

        if (captureGroup != null)
        {
            Palette palette = null;

            if (captureGroup.IsTerminal)
            {
                var currentEnclosure = _enclosureStack.Peek();
                _enclosureTerminalPropCount.TryAdd(currentEnclosure, 0);
                palette = DeterministicPalette.GetFixedRainbowPalette(_enclosureTerminalPropCount[currentEnclosure]++);
            }
            else if (TokenTypeRegistry.Palettes.TryGetValue(captureGroup.UnderlyingType, out var typePalette))
                palette = typePalette;
            else
                palette = DeterministicPalette.GetStaticPalette(new HexColor("#696969"));

            enclosure = new NamedEnclosure(_nextEnclosureOrdinal++, palette, captureGroup, nameOverride);
        }
        else
            enclosure = new Enclosure(_nextEnclosureOrdinal++);

        _enclosureStack.Push(enclosure);

        spaceDisposition ??= (captureGroup?.BaseType.IsDefined(typeof(NoSpacesAttribute)) ?? false)
            ? SpaceDisposition.NeverAddSpaceLocal
            : SpaceDisposition.DontAddSpaceBeforeNextItem;

        _spaceIsRequiredBeforeNextElementAtLevel[enclosure] = spaceDisposition.Value;

        if (captureGroup != null)
        {
            var name = nameOverride ?? captureGroup.Name;
            _regexElements.Add(new NamedGroupOpen(_orderedEnclosureStack, name, captureGroup));
        }
        else
            _regexElements.Add(new GroupOpen(_orderedEnclosureStack));
    }

    public void CloseGroup(GroupQuantifier? quantifier = null)
    {
        if (_enclosureStack.Peek() is RootEnclosure)
            throw new Exception($"No groups are available to close");

        if (_enclosureStack.Peek() is NamedEnclosure namedEnclosure)
            _regexElements.Add(new NamedGroupClose(_orderedEnclosureStack, namedEnclosure.Name, quantifier));
        else
            _regexElements.Add(new GroupClose(_orderedEnclosureStack, quantifier));

        _enclosureStack.Pop();
    }

    public void AddTextLine(string text)
    {
        AddPrecedingSpaceIfApplicable();
        _regexElements.Add(new TextLine(_orderedEnclosureStack, text));
    }

    public void AddAlternateValues(IEnumerable<string> alternatives)
        => _regexElements.Add(new AlternateValueContainer(_orderedEnclosureStack, alternatives.ToList()));

    public void AddAlternateEnumValues(EnumScalarAlternateSet enumSet)
        => _regexElements.Add(new AlternateValueEnumContainer(_orderedEnclosureStack, enumSet));

    public void AddGroupAlternativePipe()
    {
        var path = _orderedEnclosureStack;
        _regexElements.Add(new GroupAlternativePipe(_orderedEnclosureStack));
    }

    void AddPrecedingSpaceIfApplicable()
    {
        // If any parent disallows spaces globally, don't add any spaces
        if (_enclosureStack.Any(x => _spaceIsRequiredBeforeNextElementAtLevel[x] == SpaceDisposition.NeverAddSpaceGlobal))
            return;

        var currentScope = _enclosureStack.Peek();
        var groupSpaceDisposition = _spaceIsRequiredBeforeNextElementAtLevel[currentScope];

        if (groupSpaceDisposition == SpaceDisposition.AddSpaceBeforeNextItem)
            _regexElements.Add(new SpaceLine(_orderedEnclosureStack));
        else if (groupSpaceDisposition != SpaceDisposition.NeverAddSpaceLocal)
            _spaceIsRequiredBeforeNextElementAtLevel[currentScope] = SpaceDisposition.AddSpaceBeforeNextItem;
    }

    public Regex ExtractGroupRegex(RegexPropInfo group)
    {
        var firstGroupLine = _regexElements.FirstOrDefault(x => x.Enclosures.OfType<NamedEnclosure>().LastOrDefault()?.RegexPropInfo == group);
        var lastGroupLine = _regexElements.LastOrDefault(x => x.Enclosures.OfType<NamedEnclosure>().LastOrDefault()?.RegexPropInfo == group);

        if (firstGroupLine == null || lastGroupLine == null)
            return null;

        var firstLineIndex = _regexElements.IndexOf(firstGroupLine);
        var lastLineIndex = _regexElements.IndexOf(lastGroupLine);

        var groupLines = _regexElements.Skip(firstLineIndex).Take(lastLineIndex - firstLineIndex + 1).ToList();
        AddBoundaryLines(groupLines);
        var regexString = string.Join("", groupLines.Select(x => x.Regex));
        regexString = MinifyRegex(regexString);

        return new(regexString, RegexOptions.Compiled);
    }

    public List<RegexCommentedLine> GetFormattedLines(List<PropPathVariantSetWrapper> variantData)
    {
        var variantDataLookup = variantData.ToDictionary(d => d.ParentPath.PropPath);
        var alternateCounts = new Dictionary<AlternateValueEnum, int>();

        // 1. Expand containers and enrich with variant data
        List<RegexElement> expandedAndFilteredElements = [];
        foreach (var element in _regexElements)
        {
            if (element is AlternateValueEnumContainer enumContainer)
            {
                if (variantDataLookup.TryGetValue(enumContainer.NamedPath, out var wrapper))
                {
                    foreach (var variantSet in wrapper.VariantSets.Values)
                    {
                        var enumElement = enumContainer.AlternateValueEnums.First(e => e.CanonicalValue.Equals(variantSet.CanonicalValue));

                        if (variantSet.SynonymCounts.Count > 1)
                        {

                        }
                        else
                        {

                        }

                        expandedAndFilteredElements.Add(enumElement);
                        alternateCounts[enumElement] = variantSet.TotalCount;
                    }

                    int omittedCount = wrapper.AlternateCount - wrapper.VariantSets.Count;
                    if (omittedCount > 0)
                    {
                        expandedAndFilteredElements.Add(new BlankLine(enumContainer.Enclosures)
                        {
                            Comment = $"{omittedCount} omitted"
                        });
                    }
                }
            }
            else if (element is AlternateValueContainer container)
                expandedAndFilteredElements.AddRange(container.AlternateValues);
            else
                expandedAndFilteredElements.Add(element);
        }

        if (!expandedAndFilteredElements.Any()) return [];

        // 2. Add blank lines for spacing
        List<RegexElement> finalizedLines = [expandedAndFilteredElements[0]];
        for (int i = 1; i < expandedAndFilteredElements.Count; i++)
        {
            var previousLine = expandedAndFilteredElements[i - 1];
            var currentLine = expandedAndFilteredElements[i];

            bool pathChanged = currentLine.UniquePath != previousLine.UniquePath;

            if (pathChanged)
            {
                int GetEnclosureEventType(RegexElement line)
                {
                    if (line is GroupOpen or NamedGroupOpen) return 1;
                    if (line is GroupClose or NamedGroupClose) return 2;
                    return 0;
                }

                var prevEventType = GetEnclosureEventType(previousLine);
                var currentEventType = GetEnclosureEventType(currentLine);

                if (prevEventType != currentEventType || prevEventType == 0)
                {
                    int commonDepth = 0;
                    while (commonDepth < previousLine.Enclosures.Length &&
                           commonDepth < currentLine.Enclosures.Length &&
                           previousLine.Enclosures[commonDepth].Ordinal == currentLine.Enclosures[commonDepth].Ordinal)
                    {
                        commonDepth++;
                    }

                    var blankLineEnclosures = previousLine.Enclosures.Take(commonDepth).ToArray();
                    finalizedLines.Add(new BlankLine(blankLineEnclosures));
                }
            }
            finalizedLines.Add(currentLine);
        }
        AddBoundaryLines(finalizedLines);

        // 3. Prepare for formatting
        _colors = new();
        _treatments = new();
        CalculateColumnWidths(finalizedLines, alternateCounts);

        // 4. Format the lines and create RegexCommentedLine objects
        var commentedLines = new List<RegexCommentedLine>();
        for (int i = 0; i < finalizedLines.Count; i++)
        {
            RegexElement line = finalizedLines[i];
            string regexText = line.Regex;

            var spans = new List<RegexCommentedLineSpan>();
            int indentSpaces = GetIndentDepth(line) * _spacesPerIndent;
            var indentedRegex = new string(' ', indentSpaces) + regexText;
            var paddedRegex = indentedRegex.PadRight(_hashSeparatorColumn);

            var primaryContentColor = GetPrimaryContentColorForLine(line);
            var primaryContentPalette = DeterministicPalette.GetStaticPalette(new HexColor(primaryContentColor));
            var highlightTreatment = _treatments.GetRegexHighlightTreatment(line);

            string pathForRegexSpan = line.NamedPath;
            if (line is AlternateValue alt) pathForRegexSpan = $"{pathForRegexSpan}.{alt.CanonicalValue}";

            string relativePath = RegexCommentedLine.GetRelativePath(pathForRegexSpan);
            if (relativePath == null) highlightTreatment = SpanHighlightTreatment.None;

            spans.Add(new RegexCommentedLineSpan(
                SpanText: paddedRegex,
                Palette: primaryContentPalette,
                PathRelativeToRoot: relativePath,
                HighlightTreatment: highlightTreatment,
                LowlightTreatment: _treatments.CommentLowlightTreatment
            ));

            var commentPrefix = $"#{new string(' ', _hashSeparatorPadding)}";
            var hashPalette = DeterministicPalette.GetStaticPalette(new HexColor(_colors.HashSeparatorColor));

            spans.Add(new RegexCommentedLineSpan(
                SpanText: commentPrefix,
                Palette: hashPalette,
                PathRelativeToRoot: null,
                HighlightTreatment: SpanHighlightTreatment.None,
                LowlightTreatment: SpanLowlightTreatment.None
            ));

            var commentSpans = GenerateCommentSpans(line, alternateCounts);
            spans.AddRange(commentSpans);
            string commentText = commentPrefix + string.Join("", commentSpans.Select(s => s.SpanText));

            if (line is AlternateValue alternateValue)
                commentedLines.Add(new RegexCommentedAlternateLine(paddedRegex, commentText, line.NamedPath, spans, alternateValue));
            else
                commentedLines.Add(new RegexCommentedLine(paddedRegex, commentText, line.NamedPath, spans));
        }

        // 5. Add alternate ("|") prefixes
        List<RegexCommentedLine> finalResult = [];
        string currentEnclosurePath = null;
        bool isFirstInAlternateGroup = true;

        foreach (var line in commentedLines)
        {
            if (line is RegexCommentedAlternateLine altLine)
            {
                if (altLine.EnclosurePath != currentEnclosurePath)
                {
                    currentEnclosurePath = altLine.EnclosurePath;
                    isFirstInAlternateGroup = true;
                }

                string originalPaddedRegex = altLine.Regex;
                string trimmedRegex = originalPaddedRegex.Trim();
                int indentSpaces = originalPaddedRegex.Length - originalPaddedRegex.TrimStart().Length;
                string prefix = isFirstInAlternateGroup ? "   " : " | ";

                string newIndentedRegex = new string(' ', indentSpaces) + prefix + trimmedRegex;
                string newPaddedRegex = newIndentedRegex.PadRight(_hashSeparatorColumn);

                var newSpans = altLine.Spans.ToList();
                newSpans[0] = newSpans[0] with { SpanText = newPaddedRegex };

                finalResult.Add(altLine with
                {
                    Regex = newPaddedRegex,
                    FormattedText = newPaddedRegex + altLine.Comment,
                    Spans = newSpans
                });

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


    public string GetMinified()
    {
        if (!_regexElements.Any())
            return "";

        var finalizedElements = _regexElements.ToList();
        AddBoundaryLines(finalizedElements);

        return MinifyRegex(string.Join("", finalizedElements.Select(x => x.Regex)));
    }


    void AddBoundaryLines(List<RegexElement> lines)
    {
        if (_boundaryOption == BoundaryOption.Omit)
            return;

        RegexElement startBoundary = _boundaryOption == BoundaryOption.WholeWord ? new NegativeLookbehindBoundary() : new StartOfLineBoundary();
        RegexElement endBoundary = _boundaryOption == BoundaryOption.WholeWord ? new NegativeLookaheadBoundary() : new EndOfLineBoundary();

        lines.Insert(0, startBoundary);
        lines.Insert(1, new BlankLine([]));
        lines.Add(new BlankLine([]));
        lines.Add(endBoundary);
    }

    #region Formatting Helpers (Adapted from FormattedRegex)

    private void CalculateColumnWidths(List<RegexElement> lines, IReadOnlyDictionary<AlternateValueEnum, int> alternateCounts)
    {
        int maxRegexLen = lines.Any() ? lines.Max(x => (GetIndentDepth(x) * _spacesPerIndent) + x.Regex.Length) : 0;
        _hashSeparatorColumn = maxRegexLen + _hashSeparatorPadding;

        _enumBoxMetrics = new Dictionary<string, EnumBoxLayoutMetrics>();
        var enumGroups = lines.OfType<AlternateValueEnum>().GroupBy(e => e.NamedPath);
        foreach (var group in enumGroups)
        {
            int maxValueLength = group.Max(alt => alt.Comment.Length);
            int maxCountLength = group.Max(alt => alternateCounts.TryGetValue(alt, out int count) ? count.ToString().Length : 0);
            _enumBoxMetrics[group.Key] = new EnumBoxLayoutMetrics(maxValueLength, maxCountLength);
        }

        var uniquePaths = lines
            .SelectMany(l => l.PropEnclosures.Select((e, i) => l.PropEnclosures.Take(i + 1)))
            .GroupBy(p => string.Join(",", p.Select(e => e.Ordinal)))
            .Select(g => g.First())
            .Where(p => p.Any())
            .ToList();

        var boxWidths = uniquePaths.ToDictionary(p => string.Join(",", p.Select(e => e.Ordinal)), p => 0);

        foreach (var line in lines.Where(l => l.PropEnclosures.Any()))
        {
            string pathKey = string.Join(",", line.PropEnclosures.Select(e => e.Ordinal));
            int requiredWidth = 0;

            if (!string.IsNullOrEmpty(line.Comment))
            {
                int textWidth;
                switch (line)
                {
                    case AlternateValueEnum ave:
                        var metrics = _enumBoxMetrics[ave.NamedPath];
                        textWidth = metrics.MaxValueLength + 3 + metrics.MaxCountLength + 2; // " Value : Count "
                        break;
                    case BlankLine bl when bl.Comment.EndsWith("omitted"):
                        textWidth = bl.Comment.Length + 2;
                        break;
                    case AlternateValue:
                    case NamedGroupOpen:
                    case NamedGroupClose:
                    case GroupClose:
                        textWidth = line.Comment.Length + 2;
                        break;
                    default:
                        textWidth = _boxContentLeftPadding + line.Comment.Length;
                        break;
                }
                requiredWidth = line is EncloureBookend ? textWidth + 2 : textWidth + 4;
            }
            boxWidths[pathKey] = Math.Max(boxWidths[pathKey], requiredWidth);
        }

        var sortedPaths = uniquePaths.OrderByDescending(p => p.Count());
        foreach (var path in sortedPaths)
        {
            if (path.Count() <= 1) continue;
            string childPathKey = string.Join(",", path.Select(e => e.Ordinal));
            string parentPathKey = string.Join(",", path.Take(path.Count() - 1).Select(e => e.Ordinal));
            int childFootprint = boxWidths[childPathKey] + 4;
            boxWidths[parentPathKey] = Math.Max(boxWidths[parentPathKey], childFootprint);
        }

        var rootPaths = uniquePaths.Where(p => p.Count() == 1);
        _commentBoxLength = rootPaths.Any() ? rootPaths.Max(p => boxWidths[string.Join(",", p.Select(e => e.Ordinal))]) : 0;
    }

    private int GetIndentDepth(RegexElement line)
    {
        if (line.PropEnclosures.Length == 0)
            return 0;

        return line is EncloureBookend ? line.PropEnclosures.Length - 1 : line.PropEnclosures.Length;
    }

    private string GetPrimaryContentColorForLine(RegexElement line)
    {
        if (line.PropEnclosures.Length == 0)
        {
            return line switch
            {
                TextLine => _colors.UnenclosedTextLineCommentColor,
                SpaceLine => _colors.UnenclosedSpaceLineCommentColor,
                BoundaryBase => _colors.BoundaryCommentColor,
                _ => _colors.DefaultFallbackColor
            };
        }

        var currentEnclosure = line.PropEnclosures.Last();
        var palette = currentEnclosure.Palette;

        switch (line)
        {
            case NamedGroupOpen:
            case NamedGroupClose:
                return _colors.NamedGroupBookendCommentColor(palette);
            case GroupOpen:
                return DefaultWhite;
            case GroupClose:
                return _colors.GroupCloseQuantifierColor;
            case AlternateValue:
                return _colors.AlternateValueCommentColor(palette);
            case TextLine or SpaceLine or GroupAlternativePipe:
                var nearestNamedEnclosure = line.PropEnclosures.LastOrDefault(e => e is NamedEnclosure) as NamedEnclosure;
                if (nearestNamedEnclosure != null)
                {
                    return _colors.EnclosedTextColor(nearestNamedEnclosure.Palette);
                }
                return DefaultWhite;
            default:
                return DefaultWhite;
        }
    }

    private List<RegexCommentedLineSpan> GenerateCommentSpans(RegexElement line, IReadOnlyDictionary<AlternateValueEnum, int> alternateCounts)
    {
        var spans = new List<RegexCommentedLineSpan>();
        var lowlight = _treatments.CommentLowlightTreatment;

        Action<string, Palette, SpanHighlightTreatment, IEnumerable<Enclosure>> addSpanForEnclosurePath = (text, palette, highlight, enclosureScope) =>
        {
            if (string.IsNullOrEmpty(text)) return;
            string rootName = line.Enclosures.OfType<RootEnclosure>().FirstOrDefault()?.RootTypeName ?? "";
            string namedPath = string.Join('.', enclosureScope.OfType<NamedEnclosure>().Select(x => x.Name));
            string fullPath = string.IsNullOrEmpty(namedPath) ? rootName : $"{rootName}.{namedPath}";
            string relativePath = RegexCommentedLine.GetRelativePath(fullPath);

            var finalHighlight = (relativePath == null) ? SpanHighlightTreatment.None : highlight;

            spans.Add(new RegexCommentedLineSpan(text, palette, relativePath, finalHighlight, lowlight));
        };

        Action<string, Palette, bool> addSpanForCurrentLine = (text, palette, isTextSpan) =>
        {
            if (string.IsNullOrEmpty(text)) return;

            string pathForSpan = line.NamedPath;

            if (line is AlternateValue alt)
                pathForSpan = $"{pathForSpan}.{alt.CanonicalValue}";

            string relativePath = RegexCommentedLine.GetRelativePath(pathForSpan);

            var highlight = _treatments.GetCommentHighlightTreatment(line, isTextSpan);
            var finalHighlight = (relativePath == null) ? SpanHighlightTreatment.None : highlight;

            spans.Add(new RegexCommentedLineSpan(text, palette, relativePath, finalHighlight, lowlight));
        };

        var defaultWhitePalette = DeterministicPalette.GetStaticPalette(new HexColor(DefaultWhite));

        if (line.PropEnclosures.Length == 0)
        {
            var color = GetPrimaryContentColorForLine(line);
            var unenclosedPalette = DeterministicPalette.GetStaticPalette(new HexColor(color));

            spans.Add(new RegexCommentedLineSpan(line.Comment ?? string.Empty, unenclosedPalette, null, SpanHighlightTreatment.None, lowlight));
            return spans;
        }

        var parentEnclosures = line.PropEnclosures.Take(line.PropEnclosures.Length - 1).ToList();
        var currentEnclosure = line.PropEnclosures.Last();
        var chars = BoxChars.Get(currentEnclosure.Treatment);
        var palette = currentEnclosure.Palette;
        var borderPalette = DeterministicPalette.GetStaticPalette(new HexColor(_colors.GetBorderColor(currentEnclosure.Treatment, palette)));
        var borderHighlight = _treatments.GetCommentHighlightTreatment(line, isTextSpan: false);

        var currentPathParts = new List<Enclosure>();
        foreach (var parent in parentEnclosures)
        {
            currentPathParts.Add(parent);
            char wall = BoxChars.Get(parent.Treatment).Wall;
            var parentBorderPalette = DeterministicPalette.GetStaticPalette(new HexColor(_colors.GetBorderColor(parent.Treatment, parent.Palette)));
            addSpanForEnclosurePath(wall.ToString(), parentBorderPalette, borderHighlight, currentPathParts);
            addSpanForEnclosurePath(" ", defaultWhitePalette, borderHighlight, currentPathParts);
        }

        int parentDepth = parentEnclosures.Count;
        int currentLevelWidth = _commentBoxLength - (parentDepth * 4);

        if (line is EncloureBookend)
        {
            int availableWidth = currentLevelWidth - 2;
            switch (line)
            {
                case NamedGroupOpen ngo:
                    var openBookendPalette = DeterministicPalette.GetStaticPalette(new HexColor(_colors.NamedGroupBookendCommentColor(palette)));
                    string openComment = $" {ngo.Comment} ";
                    string fillerOpen = new string(chars.Top, Math.Max(0, availableWidth - openComment.Length));
                    addSpanForCurrentLine(chars.TopLeft.ToString(), borderPalette, false);
                    addSpanForCurrentLine(openComment, openBookendPalette, true);
                    addSpanForCurrentLine(fillerOpen, borderPalette, false);
                    addSpanForCurrentLine(chars.TopRight.ToString(), borderPalette, false);
                    break;
                case GroupOpen:
                    addSpanForCurrentLine(chars.TopLeft.ToString(), borderPalette, false);
                    addSpanForCurrentLine(new string(chars.Top, availableWidth), borderPalette, false);
                    addSpanForCurrentLine(chars.TopRight.ToString(), borderPalette, false);
                    break;
                case NamedGroupClose ngc:
                    var closeBookendPalette = DeterministicPalette.GetStaticPalette(new HexColor(_colors.NamedGroupBookendCommentColor(palette)));
                    string closeComment = $" {ngc.Comment} ";
                    string fillerClose = new string(chars.Bottom, Math.Max(0, availableWidth - closeComment.Length));
                    addSpanForCurrentLine(chars.BottomLeft.ToString(), borderPalette, false);
                    addSpanForCurrentLine(fillerClose, borderPalette, false);
                    addSpanForCurrentLine(closeComment, closeBookendPalette, true);
                    addSpanForCurrentLine(chars.BottomRight.ToString(), borderPalette, false);
                    break;
                case GroupClose gc:
                    var quantPalette = DeterministicPalette.GetStaticPalette(new HexColor(_colors.GroupCloseQuantifierColor));
                    string quantComment = gc.Comment != null ? $" {gc.Comment} " : "";
                    string fillerQuant = new string(chars.Bottom, Math.Max(0, availableWidth - quantComment.Length));
                    addSpanForCurrentLine(chars.BottomLeft.ToString(), borderPalette, false);
                    addSpanForCurrentLine(fillerQuant, borderPalette, false);
                    addSpanForCurrentLine(quantComment, quantPalette, true);
                    addSpanForCurrentLine(chars.BottomRight.ToString(), borderPalette, false);
                    break;
            }
        }
        else
        {
            int innerWidth = currentLevelWidth - 4;
            addSpanForEnclosurePath(chars.Wall.ToString(), borderPalette, borderHighlight, line.PropEnclosures);
            addSpanForEnclosurePath(" ", defaultWhitePalette, borderHighlight, line.PropEnclosures);

            switch (line)
            {
                case AlternateValueEnum ave when alternateCounts.TryGetValue(ave, out int count) && _enumBoxMetrics.TryGetValue(ave.NamedPath, out var metrics):
                    var altPalette = DeterministicPalette.GetStaticPalette(new HexColor(_colors.AlternateValueCommentColor(palette)));
                    string valuePart = ave.Comment.PadLeft(metrics.MaxValueLength);
                    string lineText = $"{valuePart} : {count}";

                    int contentBlockWidth = metrics.MaxValueLength + 3 + metrics.MaxCountLength;
                    int totalPad = Math.Max(0, innerWidth - contentBlockWidth);
                    string leftPad = new string(' ', totalPad / 2);
                    string rightPad = new string(' ', totalPad - (totalPad / 2));

                    string rightAlignPad = new string(' ', contentBlockWidth - lineText.Length);

                    string fullContent = $"{leftPad}{lineText}{rightAlignPad}{rightPad}";
                    addSpanForCurrentLine(fullContent, altPalette, true);
                    break;

                case BlankLine bl when !string.IsNullOrEmpty(bl.Comment) && bl.Comment.EndsWith("omitted"):
                    var omitPalette = DeterministicPalette.GetStaticPalette(new HexColor(DefaultWhite));
                    int omitPad = Math.Max(0, innerWidth - bl.Comment.Length);
                    string omitLeftPad = new string(' ', omitPad / 2);
                    string omitRightPad = new string(' ', omitPad - (omitPad / 2));
                    addSpanForCurrentLine($"{omitLeftPad}{bl.Comment}{omitRightPad}", omitPalette, true);
                    break;

                case AlternateValue av:
                    var genericAltPalette = DeterministicPalette.GetStaticPalette(new HexColor(_colors.AlternateValueCommentColor(palette)));
                    string altCommentText = $" {av.Comment} ";
                    int genericTotalPad = Math.Max(0, innerWidth - altCommentText.Length);
                    string genericLeftPad = new string(' ', genericTotalPad / 2);
                    string genericRightPad = new string(' ', genericTotalPad - (genericTotalPad / 2));
                    string genericFullContent = $"{genericLeftPad}{altCommentText}{genericRightPad}";
                    addSpanForCurrentLine(genericFullContent, genericAltPalette, true);
                    break;

                default:
                    var nearestNamedEnclosure = line.PropEnclosures.LastOrDefault(e => e is NamedEnclosure) as NamedEnclosure;
                    var contentPalette = (nearestNamedEnclosure != null)
                        ? DeterministicPalette.GetStaticPalette(new HexColor(_colors.EnclosedTextColor(nearestNamedEnclosure.Palette)))
                        : defaultWhitePalette;
                    var content = string.IsNullOrEmpty(line.Comment)
                        ? new string(' ', innerWidth)
                        : (new string(' ', _boxContentLeftPadding) + line.Comment).PadRight(innerWidth);
                    addSpanForCurrentLine(content, contentPalette, true);
                    break;
            }

            addSpanForEnclosurePath(" ", defaultWhitePalette, borderHighlight, line.PropEnclosures);
            addSpanForEnclosurePath(chars.Wall.ToString(), borderPalette, borderHighlight, line.PropEnclosures);
        }

        var reversedParents = parentEnclosures.AsEnumerable().Reverse().ToList();
        for (int j = 0; j < reversedParents.Count(); j++)
        {
            var parent = reversedParents[j];
            var wallPathScope = parentEnclosures.Take(parentEnclosures.Count - j).ToList();
            char wall = BoxChars.Get(parent.Treatment).Wall;
            var parentBorderPalette = DeterministicPalette.GetStaticPalette(new HexColor(_colors.GetBorderColor(parent.Treatment, parent.Palette)));
            addSpanForEnclosurePath(" ", defaultWhitePalette, borderHighlight, wallPathScope);
            addSpanForEnclosurePath(wall.ToString(), parentBorderPalette, borderHighlight, wallPathScope);
        }

        return spans;
    }

    private string MinifyRegex(string pattern)
    {
        string placeholder = Guid.NewGuid().ToString();
        string protectedPattern = pattern.Replace("[ ]", placeholder);
        string strippedPattern = Regex.Replace(protectedPattern, @"\s", "");
        return strippedPattern.Replace(placeholder, " ");
    }

    private record BoxCharSet(char TopLeft, char TopRight, char BottomLeft, char BottomRight, char Top, char Bottom, char Wall);

    private static class BoxChars
    {
        // Unicode escape sequences for box-drawing characters.
        // This keeps the source file ASCII-safe and Git-friendly.
        private static readonly BoxCharSet Closed = new(
            TopLeft: '\u250C', // ┌
            TopRight: '\u2510', // ┐
            BottomLeft: '\u2514', // └
            BottomRight: '\u2518', // ┘
            Top: '\u2500', // ─
            Bottom: '\u2500', // ─
            Wall: '\u2502'  // │
        );

        private static readonly BoxCharSet Dashed = new(
            TopLeft: '\u250C', // ┌
            TopRight: '\u2510', // ┐
            BottomLeft: '\u2514', // └
            BottomRight: '\u2518', // ┘
            Top: '\u2500', // ─
            Bottom: '\u2500', // ─
            Wall: '\u250A'  // ┆
        );

        private static readonly BoxCharSet Brace = new(
            TopLeft: '\u256D', // ╭
            TopRight: '\u256E', // ╮
            BottomLeft: '\u2570', // ╰
            BottomRight: '\u256F', // ╯
            Top: ' ',      //
            Bottom: ' ',      //
            Wall: '\u2506'  // ┊
        );

        public static BoxCharSet Get(GroupBorderTreatment treatment) => treatment switch
        {
            GroupBorderTreatment.ClosedBox => Closed,
            GroupBorderTreatment.DashedBox => Dashed,
            GroupBorderTreatment.Brace => Brace,
            _ => Closed,
        };
    }

    #endregion
}

public enum SpaceDisposition
{
    NeverAddSpaceLocal,
    NeverAddSpaceGlobal,
    DontAddSpaceBeforeNextItem,
    AddSpaceBeforeNextItem,
}