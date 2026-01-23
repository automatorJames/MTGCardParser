namespace MTGPlexer.TokenEditor;

public class EditorTokenUnit
{
    private const string SaveFileInNamespace = "MTGPlexer.TokenUnits";

    private static readonly Regex TemplateSplitPattern = new Regex(
        @"(?<Method>@(?<MethodName>\w+)\((?:\s*(?<Arg>[^,)]+)\s*(?:,|$|(?=\)))\s*)*\))|(?<Type>@(?:(?<Wrapper>\w+)<(?<Base>\w+)>|(?<Base>\w+)))|(?<Plain>(?:(?!@\w).)+)",
        RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

    public ProcessedLine LineMetadata { get; }
    public Type TokenUnitType { get; } = typeof(TokenUnit);
    public List<EditorSnippet> Snippets { get; private set; } = [];

    // Naming State
    private string _suggestedClassName = "";
    public string ManualClassName { get; private set; }
    public string ClassName => ManualClassName ?? _suggestedClassName;

    // Consumables for the UI
    public string RawTemplate { get; private set; } = "";
    public string RenderedRegex { get; private set; } = "";
    public string ClassStringForSavingToFile { get; private set; } = "";
    public string ClassStringForDisplayingHtml { get; private set; } = "";
    public List<RegexSegment> RegexSegments { get; private set; } = [];
    public List<TextSegment> TextSegments { get; private set; } = [];
    public List<Match> CurrentMatches { get; private set; } = [];

    public record RegexSegment(string Text, string Color);
    public record TextSegment(string Text, string Color, string UnderlineClass);
    private enum MatchStatus { None, Partial, Full }

    public EditorTokenUnit(ProcessedLine lineMetadata)
    {
        LineMetadata = lineMetadata;
        _suggestedClassName = $"New{TokenUnitType.Name}";
        Update(string.Empty);
    }

    public EditorPropertySnippet this[string id] =>
        Snippets.OfType<EditorPropertySnippet>().FirstOrDefault(x => x.Id == id);

    /// <summary>
    /// The single entry point for updating the model state.
    /// </summary>
    public void Update(string templateString = null, string preferredClassName = null)
    {
        // 1. If the template changed, rebuild snippets and the suggestion
        if (templateString != null)
        {
            RawTemplate = templateString;
            DigestTemplateStringToSnippets(templateString);
            _suggestedClassName = GetSuggestedClassName();
        }

        // 2. If a preferred name is provided, it's a manual edit. 
        // If the string is empty/whitespace, we revert to using suggestions.
        if (preferredClassName != null)
        {
            ManualClassName = string.IsNullOrWhiteSpace(preferredClassName) ? null : preferredClassName;
        }

        // 3. Generate Regex
        RenderedRegex = CompositionFactory.GetComposedString(Snippets.Select(x => x.GetRegexSegment()), TokenUnitType);

        // 4. Analysis & Matching
        ParseRegexSegments();
        PerformMatching();
        GenerateTextSegments();

        // 5. Generate C# Previews
        ClassStringForSavingToFile = GetClassStringForSavingToFile();
        ClassStringForDisplayingHtml = GetClassStringForDisplayingHtml();
    }

    private void PerformMatching()
    {
        CurrentMatches.Clear();
        if (string.IsNullOrWhiteSpace(RenderedRegex)) return;

        try
        {
            CurrentMatches = Regex.Matches(LineMetadata.SourceText.FormattedText, RenderedRegex)
                .Cast<Match>()
                .ToList();
        }
        catch { /* Invalid Regex during typing */ }
    }

    private void ParseRegexSegments()
    {
        RegexSegments.Clear();
        if (string.IsNullOrEmpty(RenderedRegex)) return;

        int depth = 0;
        int lastPos = 0;

        for (int i = 0; i < RenderedRegex.Length; i++)
        {
            if (RenderedRegex[i] == '\\') { i++; continue; }
            if (RenderedRegex[i] == '(')
            {
                if (depth == 0)
                {
                    if (i > lastPos)
                        RegexSegments.Add(new(RenderedRegex.Substring(lastPos, i - lastPos), "var(--syntax-default)"));

                    lastPos = i;
                }

                depth++;
            }
            else if (RenderedRegex[i] == ')')
            {
                depth--;

                if (depth == 0)
                {
                    var groupText = RenderedRegex.Substring(lastPos, i - lastPos + 1);
                    var match = Regex.Match(groupText, @"^\(\?<(?<name>[a-zA-Z0-9_]+)>");
                    string color = "var(--syntax-default)";
                    if (match.Success)
                    {
                        var name = match.Groups["name"].Value;
                        if (TokenTypeRegistry.NameToType.TryGetValue(name, out var type))
                            color = DeterministicPalette.TypePaletteSet[type].Normal;
                    }
                    RegexSegments.Add(new(groupText, color));
                    lastPos = i + 1;
                }
            }
        }

        if (lastPos < RenderedRegex.Length)
            RegexSegments.Add(new(RenderedRegex.Substring(lastPos), "var(--syntax-default)"));
    }

    private void GenerateTextSegments()
    {
        var rawSegments = new List<TextSegment>();
        string text = LineMetadata.SourceText.FormattedText;
        if (string.IsNullOrEmpty(text)) { TextSegments = []; return; }

        var charStatus = new MatchStatus[text.Length];
        Array.Fill(charStatus, MatchStatus.None);

        var words = new List<(int Start, int End)>();
        int? wordStart = null;
        for (int i = 0; i <= text.Length; i++)
        {
            bool isWordChar = i < text.Length && !char.IsWhiteSpace(text[i]);

            if (isWordChar && wordStart == null) 
                wordStart = i;

            else if (!isWordChar && wordStart != null)
            {
                words.Add((wordStart.Value, i - 1));
                wordStart = null;
            }
        }

        foreach (var m in CurrentMatches)
        {
            int mStart = m.Index;
            int mEnd = m.Index + m.Length - 1;
            var overlappingWords = words.Where(w => mStart <= w.End && mEnd >= w.Start);

            foreach (var word in overlappingWords)
            {
                int strippedEnd = (text[word.End] == '.') ? word.End - 1 : word.End;
                bool coversFull = (mStart <= word.Start && mEnd >= word.End);
                bool coversStripped = (mStart <= word.Start && mEnd == strippedEnd && strippedEnd < word.End);

                if (coversFull || coversStripped)
                    for (int k = Math.Max(mStart, word.Start); k <= Math.Min(mEnd, word.End); k++)
                        charStatus[k] = MatchStatus.Full;
                else
                    for (int k = Math.Max(mStart, word.Start); k <= Math.Min(mEnd, word.End); k++)
                        if (charStatus[k] == MatchStatus.None) charStatus[k] = MatchStatus.Partial;
            }

            for (int k = mStart; k <= mEnd; k++)
            {
                if (charStatus[k] == MatchStatus.None)
                {
                    bool leftFull = k > 0 && charStatus[k - 1] == MatchStatus.Full;
                    bool rightFull = k < text.Length - 1 && charStatus[k + 1] == MatchStatus.Full;
                    if (leftFull && rightFull && char.IsWhiteSpace(text[k])) charStatus[k] = MatchStatus.Full;
                    else charStatus[k] = MatchStatus.Partial;
                }
            }
        }

        for (int i = 0; i < text.Length; i++)
        {
            string color;
            string underlineClass = "";
            MatchStatus status = charStatus[i];

            if (status == MatchStatus.Full)
            {
                color = "var(--match-full-text)";
                underlineClass = "full-match";
            }
            else if (status == MatchStatus.Partial)
            {
                color = "var(--match-partial-text)";
                underlineClass = "partial-match";
            }
            else
            {
                var span = LineMetadata.SpanRoots.FirstOrDefault(
                    sr => i >= sr.RootToken.Match.RootMatch.Index 
                    && i < sr.RootToken.Match.RootMatch.Index + sr.RootToken.Match.RootMatch.Length);

                color = (span?.RootToken.Type == typeof(DefaultUnmatchedString)) ? "var(--unmatched-default)" : (span?.Palette.Normal ?? "var(--unmatched-default)");
            }
            rawSegments.Add(new TextSegment(text[i].ToString(), color, underlineClass));
        }

        TextSegments = CollapseSegments(rawSegments);
    }

    private List<TextSegment> CollapseSegments(List<TextSegment> source)
    {
        if (source.Count == 0) return source;
        var result = new List<TextSegment>();
        var current = source[0];

        for (int i = 1; i < source.Count; i++)
        {
            if (source[i].Color == current.Color && source[i].UnderlineClass == current.UnderlineClass)
                current = current with { Text = current.Text + source[i].Text };
            else
            {
                result.Add(current);
                current = source[i];
            }
        }
        result.Add(current);
        return result;
    }

    private void DigestTemplateStringToSnippets(string templateString)
    {
        Dictionary<string, int> snippetNameOccurrenceCount = [];
        List<EditorSnippet> list = [];
        var matches = TemplateSplitPattern.Matches(templateString);

        string GetId(string name) => snippetNameOccurrenceCount.TryAdd(name, 0) ? name : $"{name}-{++snippetNameOccurrenceCount[name]}";

        foreach (Match match in matches)
        {
            string snippet = match.Value.Trim();

            if (string.IsNullOrEmpty(snippet)) 
                continue;

            if (match.Groups["Method"].Success)
            {
                var name = match.Groups["MethodName"].Value;
                var args = match.Groups["Arg"].Captures.Cast<Capture>().Select(c => c.Value.Trim()).ToArray();

                if (Enum.TryParse<ShortcutSnippetMethod>(name, out var parsedMethodType))
                    list.Add(new EditorMethodSnippet(parsedMethodType, args, GetId(name)));
            }
            else if (match.Groups["Type"].Success)
            {
                var wrapper = match.Groups["Wrapper"].Value;
                var baseType = match.Groups["Base"].Value;
                var name = string.IsNullOrEmpty(wrapper) ? baseType : $"{wrapper}<{baseType}>";
                XOfType xOfType = XOfType.None;

                if (!string.IsNullOrEmpty(wrapper) && !Enum.TryParse<XOfType>(wrapper, out xOfType))
                    continue;

                if (TokenTypeRegistry.NameToType.TryGetValue(baseType, out Type parsedBaseType))
                    list.Add(new EditorPropertySnippet(parsedBaseType, xOfType, GetId(name)));
            }
            else if (match.Groups["Plain"].Success)
            {
                var text = match.Value.Trim();
                list.Add(new EditorTextSnippet(text, GetId(text)));
            }
        }
        Snippets = list;
    }

    private string GetSuggestedClassName()
    {
        string str = "";
        foreach (var snippet in Snippets)
        {
            if (snippet is EditorPropertySnippet propertySnippet)
                str += propertySnippet.PropertyNameRepresentation;
            else if (snippet is EditorTextSnippet textSnippet)
            {
                var rawTextParts = textSnippet.Text
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Regex.Replace(x, @"[^\w]+", ""))
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => char.ToUpper(x[0]) + x[1..].ToLower());
                str += string.Join("", rawTextParts);
            }
        }
        return string.IsNullOrEmpty(str) ? $"New{TokenUnitType.Name}" : str;
    }

    public void RemoveSnippet(string snippetId)
    {
        Snippets.RemoveAll(s => s.Id == snippetId);
        Update(GetTemplateString());
    }

    public string GetTemplateString() => string.Join(' ', Snippets.Select(x => x.EditorRepresentation));

    private string GetClassStringForSavingToFile() =>
        $$"""
        namespace {{SaveFileInNamespace}};

        public class {{ClassName}} : {{nameof(TokenUnit)}}
        {
            protected override Snippet[] Snippets => [{{string.Join(", ", Snippets.Select(x => x.ParameterRepresentation))}}];

            {{string.Join("\r\n    ", Snippets.OfType<EditorPropertySnippet>().Select(x => x.GetPropertyLineRepresentation()))}}
        }
        """;

    private string GetClassStringForDisplayingHtml()
    {
        var classDeclaration = $"{Span("public class")} {Span(ClassName, SpanClass.type)} {Span(":", SpanClass.identifier)} {Span(TokenUnitType.Name, SpanClass.type)}";
        var snippetSection = $"{Span("protected override")} {Span("Snippet", SpanClass.type)}{Span("[]")} {Span("Snippets =>", SpanClass.identifier)} {Span("[", SpanClass.identifier)}"
            + string.Join(", ", Snippets.Select(x => x.GetParameterHtmlRepresentation()))
            + $"{Span("]", SpanClass.identifier)};";

        var propDeclarations = Snippets.OfType<EditorPropertySnippet>().Select(x => x.GetPropertyLineHtmlRepresentation());

        var styled = $$"""
            {{classDeclaration}}
            {
                {{snippetSection}}

                {{string.Join("\r\n    ", propDeclarations)}}
            }
            """;
        return styled.Replace("  ", "&nbsp;&nbsp;").Replace("\r\n", "<br>");
    }

    private static string Span(string content, SpanClass spanClass = SpanClass.keyword) => $"<span class=\"{spanClass}\">{content}</span>";
}