using Microsoft.Identity.Client;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;

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
    private const int _hashSeparatorPadding = 4;
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

        return new(regexString, RegexOptions.Compiled);
    }

    public List<RegexCommentedLine> GetFormattedLines(List<PropPathSynonymSetWrapper> synonymData)
    {
        var synonymDataLookup = synonymData.ToDictionary(d => d.ParentPath.PropPath);
        var alternateCounts = new Dictionary<AlternateValueEnum, int>();

        // 1. Expand containers and enrich with variant data
        List<RegexElement> expandedAndFilteredElements = [];
        foreach (var element in _regexElements)
        {
            if (element is AlternateValueEnumContainer enumContainer)
            {
                // After finishing a set of synonyms, we should add a spacer before the next alternate;
                // This buffer holds that spacer if it's necessary to use.
                SynonymTrailingSpacer spacerBuffer = null;

                if (synonymDataLookup.TryGetValue(enumContainer.NamedPath, out var wrapper))
                {
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
                            // If more than one synonym variant was captured for the current enum,
                            // add a header followed by each synonym

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
                            // Otherwise add a single line to represent the alternate value with the matched synonym as the display name
                            enumElement.DisplayOverrideName = synonymSet.SynonymCounts.First().Key;
                            expandedAndFilteredElements.Add(enumElement);
                            alternateCounts[enumElement] = synonymSet.TotalCount;
                        }
                    }

                    int omittedCount = wrapper.AlternateCount - wrapper.SynonymSets.Count;
                    if (omittedCount > 0)
                    {
                        expandedAndFilteredElements.Add(new BlankLine(enumContainer.Enclosures)
                        {
                            Comment = $"{omittedCount} omitted",
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
            string regexText = GetFinalRegexString(line);

            var spans = new List<RegexCommentedLineSpan>();
            int indentSpaces = GetIndentDepth(line) * _spacesPerIndent;
            var indentedRegex = new string(' ', indentSpaces) + regexText;
            var paddedRegex = indentedRegex.PadRight(_hashSeparatorColumn);

            var primaryContentColor = GetPrimaryContentColorForLine(line);
            var primaryContentPalette = DeterministicPalette.GetStaticPalette(new HexColor(primaryContentColor));
            var highlightTreatment = _treatments.GetRegexHighlightTreatment(line);

            string pathForRegexSpan = line.NamedPath;

            if (line is SynonymValueEnum syn)
                pathForRegexSpan = $"{pathForRegexSpan}.{syn.CanonicalValue}";
            else if (line is AlternateValueEnum altValEnum)
                pathForRegexSpan = $"{pathForRegexSpan}.{altValEnum.CanonicalValue}";
            else if (line is AlternateValue alt)
                pathForRegexSpan = $"{pathForRegexSpan}.{alt.CanonicalValue}";

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

            if (line is AlternateValue alternateValue && line is not SynonymSetHeader && line is not SynonymTrailingSpacer)
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

        return string.Join("", finalizedElements.Select(x => x.Regex)).Replace("[ ]", " ");
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

    /// <summary>
    /// Gets the final, display-ready regex string for a given line. This method is the single source of truth
    /// for the text that appears on the left side of the '#' separator. It handles special synonym display
    /// logic and the substitution of spaces with '[ ]' for AlternateValue lines.
    /// </summary>
    private string GetFinalRegexString(RegexElement line)
    {
        // Determine the base string from the line type
        string regexText =
            line is SynonymSetHeader or SynonymTrailingSpacer ? ""
            : line is SynonymValueEnum synonymValueEnum ? synonymValueEnum.CanonicalValue.ToString()
            : line is AlternateValueEnum alternateValueEnum ? alternateValueEnum.DisplayOverrideName ?? alternateValueEnum.Regex
            : line.Regex;

        // The 'TextLine' constructor already replaces spaces, so we only need to handle 'AlternateValue' here.
        if (line is AlternateValue)
        {
            return regexText.Replace(" ", "[ ]");
        }

        return regexText;
    }

    private void CalculateColumnWidths(List<RegexElement> lines, IReadOnlyDictionary<AlternateValueEnum, int> alternateCounts)
    {
        // Local function to determine the final rendered length of the regex part of a line,
        // including indent and any prefixes that will be added during formatting. This is crucial
        // for correctly positioning the '#' separator column.
        int getRenderedLineLength(RegexElement line)
        {
            // Start with the indent depth.
            int length = GetIndentDepth(line) * _spacesPerIndent;

            // Add the length of the final regex string content.
            length += GetFinalRegexString(line).Length;

            // For AlternateValue lines, a 3-character prefix (" | " or "   ") is added
            // during the final formatting step. We must account for it here.
            if (line is AlternateValue)
            {
                length += 3;
            }
            return length;
        }

        // To find the position for the '#', we find the longest rendered regex line and add padding.
        int maxRegexLen = lines.Any() ? lines.Max(getRenderedLineLength) : 0;
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

        // local helper
        void AddSpanForEnclosurePath(string text, Palette palette, SpanHighlightTreatment highlight, IEnumerable<Enclosure> enclosureScope)
        {
            if (string.IsNullOrEmpty(text)) return;
            string rootName = line.Enclosures.OfType<RootEnclosure>().FirstOrDefault()?.RootTypeName ?? "";
            string namedPath = string.Join('.', enclosureScope.OfType<NamedEnclosure>().Select(x => x.Name));
            string fullPath = string.IsNullOrEmpty(namedPath) ? rootName : $"{rootName}.{namedPath}";
            string relativePath = RegexCommentedLine.GetRelativePath(fullPath);

            var finalHighlight = (relativePath == null) ? SpanHighlightTreatment.None : highlight;

            spans.Add(new RegexCommentedLineSpan(text, palette, relativePath, finalHighlight, lowlight));
        }

        // local helper
        void AddSpanForCurrentLine(string text, Palette palette, bool isTextSpan)
        {
            if (string.IsNullOrEmpty(text)) return;

            string pathForSpan = line.NamedPath;

            if (line is SynonymValueEnum synEnum)
                pathForSpan = $"{pathForSpan}.{synEnum.CanonicalParent.CanonicalValue}.{synEnum.CanonicalValue}";
            else if (line is AlternateValueEnum altEnum)
                pathForSpan = $"{pathForSpan}.{altEnum.CanonicalValue}";
            else if (line is AlternateValue alt)
                pathForSpan = $"{pathForSpan}.{alt.CanonicalValue}";

            string relativePath = RegexCommentedLine.GetRelativePath(pathForSpan);

            var highlight = _treatments.GetCommentHighlightTreatment(line, isTextSpan);
            var finalHighlight = (relativePath == null) ? SpanHighlightTreatment.None : highlight;

            spans.Add(new RegexCommentedLineSpan(text, palette, relativePath, finalHighlight, lowlight));
        }

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
            AddSpanForEnclosurePath(wall.ToString(), parentBorderPalette, borderHighlight, currentPathParts);
            AddSpanForEnclosurePath(" ", defaultWhitePalette, borderHighlight, currentPathParts);
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
                    AddSpanForCurrentLine(chars.TopLeft.ToString(), borderPalette, false);
                    AddSpanForCurrentLine(openComment, openBookendPalette, true);
                    AddSpanForCurrentLine(fillerOpen, borderPalette, false);
                    AddSpanForCurrentLine(chars.TopRight.ToString(), borderPalette, false);
                    break;
                case GroupOpen:
                    AddSpanForCurrentLine(chars.TopLeft.ToString(), borderPalette, false);
                    AddSpanForCurrentLine(new string(chars.Top, availableWidth), borderPalette, false);
                    AddSpanForCurrentLine(chars.TopRight.ToString(), borderPalette, false);
                    break;
                case NamedGroupClose ngc:
                    var closeBookendPalette = DeterministicPalette.GetStaticPalette(new HexColor(_colors.NamedGroupBookendCommentColor(palette)));
                    string closeComment = $" {ngc.Comment} ";
                    string fillerClose = new string(chars.Bottom, Math.Max(0, availableWidth - closeComment.Length));
                    AddSpanForCurrentLine(chars.BottomLeft.ToString(), borderPalette, false);
                    AddSpanForCurrentLine(fillerClose, borderPalette, false);
                    AddSpanForCurrentLine(closeComment, closeBookendPalette, true);
                    AddSpanForCurrentLine(chars.BottomRight.ToString(), borderPalette, false);
                    break;
                case GroupClose gc:
                    var quantPalette = DeterministicPalette.GetStaticPalette(new HexColor(_colors.GroupCloseQuantifierColor));
                    string quantComment = gc.Comment != null ? $" {gc.Comment} " : "";
                    string fillerQuant = new string(chars.Bottom, Math.Max(0, availableWidth - quantComment.Length));
                    AddSpanForCurrentLine(chars.BottomLeft.ToString(), borderPalette, false);
                    AddSpanForCurrentLine(fillerQuant, borderPalette, false);
                    AddSpanForCurrentLine(quantComment, quantPalette, true);
                    AddSpanForCurrentLine(chars.BottomRight.ToString(), borderPalette, false);
                    break;
            }
        }
        else
        {
            int innerWidth = currentLevelWidth - 4;
            AddSpanForEnclosurePath(chars.Wall.ToString(), borderPalette, borderHighlight, line.PropEnclosures);
            AddSpanForEnclosurePath(" ", defaultWhitePalette, borderHighlight, line.PropEnclosures);

            switch (line)
            {
                case SynonymTrailingSpacer:
                    {
                        string lineSpacer = new string('-', innerWidth);
                        AddSpanForCurrentLine(lineSpacer, palette, true);
                        break;
                    }

                case AlternateValueEnum ave when alternateCounts.TryGetValue(ave, out int count) && _enumBoxMetrics.TryGetValue(ave.NamedPath, out var metrics):
                    {
                        var altPalette = ave is SynonymValueEnum
                            ? DeterministicPalette.GetStaticPalette(new HexColor(_colors.SynonymValueCommentColor(palette)))
                            : DeterministicPalette.GetStaticPalette(new HexColor(_colors.AlternateValueCommentColor(palette)));

                        string valuePart = ave.Comment;
                        string valuePadded = valuePart.PadLeft(metrics.MaxValueLength);
                        string lineText = $"{valuePadded} : {count}";
                        int contentBlockWidth = metrics.MaxValueLength + 3 + metrics.MaxCountLength;
                        int totalPad = Math.Max(0, innerWidth - contentBlockWidth);
                        string leftPad = new string(' ', totalPad / 2);
                        string rightPad = new string(' ', totalPad - (totalPad / 2));
                        string rightAlignPad = new string(' ', contentBlockWidth - lineText.Length);
                        string fullContent = $"{leftPad}{lineText}{rightAlignPad}{rightPad}";
                        AddSpanForCurrentLine(fullContent, altPalette, true);
                        break;
                    }

                case BlankLine bl when !string.IsNullOrEmpty(bl.Comment) && bl.Comment.EndsWith("omitted"):
                    var omitPalette = DeterministicPalette.GetStaticPalette(new HexColor(_colors.OmittedEnumCountColor));
                    int omitPad = Math.Max(0, innerWidth - bl.Comment.Length);
                    string omitLeftPad = new string(' ', omitPad / 2);
                    string omitRightPad = new string(' ', omitPad - (omitPad / 2));
                    AddSpanForCurrentLine($"{omitLeftPad}{bl.Comment}{omitRightPad}", omitPalette, true);
                    break;

                case AlternateValue av:
                    var genericAltPalette = DeterministicPalette.GetStaticPalette(new HexColor(_colors.AlternateValueCommentColor(palette)));
                    string altCommentText = $" {av.Comment} ";
                    int genericTotalPad = Math.Max(0, innerWidth - altCommentText.Length);
                    string genericLeftPad = new string(' ', genericTotalPad / 2);
                    string genericRightPad = new string(' ', genericTotalPad - (genericTotalPad / 2));
                    string genericFullContent = $"{genericLeftPad}{altCommentText}{genericRightPad}";
                    AddSpanForCurrentLine(genericFullContent, genericAltPalette, true);
                    break;

                default:
                    var nearestNamedEnclosure = line.PropEnclosures.LastOrDefault(e => e is NamedEnclosure) as NamedEnclosure;
                    var contentPalette = (nearestNamedEnclosure != null)
                        ? DeterministicPalette.GetStaticPalette(new HexColor(_colors.EnclosedTextColor(nearestNamedEnclosure.Palette)))
                        : defaultWhitePalette;
                    var content = string.IsNullOrEmpty(line.Comment)
                        ? new string(' ', innerWidth)
                        : (new string(' ', _boxContentLeftPadding) + line.Comment).PadRight(innerWidth);
                    AddSpanForCurrentLine(content, contentPalette, true);
                    break;
            }

            AddSpanForEnclosurePath(" ", defaultWhitePalette, borderHighlight, line.PropEnclosures);
            AddSpanForEnclosurePath(chars.Wall.ToString(), borderPalette, borderHighlight, line.PropEnclosures);
        }

        var reversedParents = parentEnclosures.AsEnumerable().Reverse().ToList();
        for (int j = 0; j < reversedParents.Count(); j++)
        {
            var parent = reversedParents[j];
            var wallPathScope = parentEnclosures.Take(parentEnclosures.Count - j).ToList();
            char wall = BoxChars.Get(parent.Treatment).Wall;
            var parentBorderPalette = DeterministicPalette.GetStaticPalette(new HexColor(_colors.GetBorderColor(parent.Treatment, parent.Palette)));
            AddSpanForEnclosurePath(" ", defaultWhitePalette, borderHighlight, wallPathScope);
            AddSpanForEnclosurePath(wall.ToString(), parentBorderPalette, borderHighlight, wallPathScope);
        }

        return spans;
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
}

public enum SpaceDisposition
{
    NeverAddSpaceLocal,
    NeverAddSpaceGlobal,
    DontAddSpaceBeforeNextItem,
    AddSpaceBeforeNextItem,
}