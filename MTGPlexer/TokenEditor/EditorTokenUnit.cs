using System;

namespace MTGPlexer.TokenEditor;

public class EditorTokenUnit
{
    const string SaveFileInNamespace = "MTGPlexer.TokenUnits";

    public ProcessedLine LineMetadata { get; }
    public Type TokenUnitType { get; } = typeof(TokenUnit);
    public List<EditorSnippet> Snippets { get; private set; } = [];

    string _suggestedClassName = "";
    public string ManualClassName { get; private set; }
    public string ClassName => ManualClassName ?? _suggestedClassName;

    public string RawTemplate => string.Concat(Snippets.Select(s => s.EditorRepresentation));
    public string RenderedRegex { get; private set; } = "";
    public string ClassStringForSavingToFile { get; private set; } = "";
    public string ClassStringForDisplayingHtml { get; private set; } = "";
    public List<RegexStyledRun> RegexRuns { get; private set; } = [];
    public List<TextStyledRun> TextRuns { get; private set; } = [];
    public List<Match> CurrentMatches { get; private set; } = [];

    IEnumerable<EditorSnippet> _nonEmptySnippets =>
        Snippets.Where(x => x is not EditorTextSnippet textSnippet || !string.IsNullOrWhiteSpace(textSnippet.RawText));

    public EditorTokenUnit(ProcessedLine lineMetadata)
    {
        LineMetadata = lineMetadata;
        _suggestedClassName = $"New{TokenUnitType.Name}";
        Update([]);
    }

    public EditorPropertySnippet this[string id] =>
        Snippets.OfType<EditorPropertySnippet>().FirstOrDefault(x => x.Id == id);

    public void Update(List<TemplateFragment> fragments = null, string preferredClassName = null)
    {
        if (fragments != null)
        {
            SyncSnippetsFromFragments(fragments);
            _suggestedClassName = GetSuggestedClassName();
        }

        if (preferredClassName != null)
            ManualClassName = string.IsNullOrWhiteSpace(preferredClassName) ? null : preferredClassName;

        RenderedRegex = CompositionFactory.GetComposedString(_nonEmptySnippets.Select(x => x.GetRegexSegment()), TokenUnitType);

        ParseRegexSegments();
        PerformMatching();
        GenerateTextStyledRuns();

        ClassStringForSavingToFile = GetClassStringForSavingToFile();
        ClassStringForDisplayingHtml = GetClassStringForDisplayingHtml();
    }

    private void SyncSnippetsFromFragments(List<TemplateFragment> fragments)
    {
        var newSnippets = new List<EditorSnippet>();
        var existingSnippetMap = Snippets.Where(s => s.Id != null).ToDictionary(s => s.Id);
        int autoIdCounter = 0;

        foreach (var frag in fragments)
        {
            if (frag.IsPill)
            {
                // If we already have this snippet in our internal list, keep it.
                // This preserves its XOfType, Proptions, and other internal states.
                if (frag.Id != null && existingSnippetMap.TryGetValue(frag.Id, out var existing))
                {
                    newSnippets.Add(existing);
                }
                else
                {
                    // Brand new pill (e.g. just inserted from autocomplete)
                    var newSnippet = CreateSnippetFromFragment(frag);
                    if (newSnippet != null) newSnippets.Add(newSnippet);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(frag.Text)) continue;
                newSnippets.Add(new EditorTextSnippet(frag.Text, $"txt-{autoIdCounter++}"));
            }
        }

        Snippets = CollapseTextSnippets(newSnippets);
    }

    private EditorSnippet CreateSnippetFromFragment(TemplateFragment frag)
    {
        if (!frag.IsPill) return new EditorTextSnippet(frag.Text, frag.Id);

        if (!string.IsNullOrEmpty(frag.MethodName))
            if (Enum.TryParse<ShortcutSnippetMethod>(frag.MethodName, out var methodType))
                return new EditorMethodSnippet(methodType, frag.Args ?? [], frag.Id);

        if (!string.IsNullOrEmpty(frag.TypeName))
            if (TokenTypeRegistry.NameToType.TryGetValue(frag.TypeName, out Type parsedBaseType))
                return new EditorPropertySnippet(parsedBaseType, XOfType.None, frag.Id);

        return new EditorTextSnippet(frag.Text, frag.Id);
    }

    private List<EditorSnippet> CollapseTextSnippets(List<EditorSnippet> source)
    {
        if (source.Count == 0) return source;

        var result = new List<EditorSnippet>();
        EditorTextSnippet currentText = null;

        foreach (var snippet in source)
        {
            if (snippet is EditorTextSnippet textSnippet)
            {
                if (currentText == null) currentText = textSnippet;
                else currentText = currentText with { RawText = currentText.RawText + textSnippet.RawText };
            }
            else
            {
                if (currentText != null) { result.Add(currentText); currentText = null; }
                result.Add(snippet);
            }
        }

        if (currentText != null) result.Add(currentText);
        return result;
    }

    public List<TemplateFragment> GetTemplateFragments()
    {
        return Snippets.Select(s => {
            string typeName = (s as EditorPropertySnippet)?.BasePropertyType.Name;
            string methodName = (s as EditorMethodSnippet)?.MethodType.ToString();
            string[] args = (s as EditorMethodSnippet)?.Args;

            return new TemplateFragment(
                Text: s.EditorRepresentation,
                Id: s.Id,
                IsPill: s is EditorBlockSnippet,
                TypeName: typeName,
                MethodName: methodName,
                Args: args
            );
        }).ToList();
    }

    void PerformMatching()
    {
        CurrentMatches.Clear();
        if (string.IsNullOrEmpty(RenderedRegex)) return;

        try
        {
            CurrentMatches = Regex.Matches(LineMetadata.SourceText.FormattedText, RenderedRegex)
                .Cast<Match>()
                .ToList();
        }
        catch { }
    }

    void ParseRegexSegments()
    {
        RegexRuns.Clear();
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
                        RegexRuns.Add(new(RenderedRegex.Substring(lastPos, i - lastPos), "var(--syntax-default)"));
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

                    RegexRuns.Add(new(groupText, color));
                    lastPos = i + 1;
                }
            }
        }

        if (lastPos < RenderedRegex.Length)
            RegexRuns.Add(new(RenderedRegex.Substring(lastPos), "var(--syntax-default)"));
    }

    void GenerateTextStyledRuns()
    {
        var rawSegments = new List<TextStyledRun>();
        string text = LineMetadata.SourceText.FormattedText;

        if (string.IsNullOrEmpty(text)) { TextRuns = []; return; }

        var charStatus = new MatchStatus[text.Length];
        Array.Fill(charStatus, MatchStatus.None);

        var words = new List<(int Start, int End)>();
        int? wordStart = null;

        for (int i = 0; i <= text.Length; i++)
        {
            bool isWordChar = i < text.Length && !char.IsWhiteSpace(text[i]);
            if (isWordChar && wordStart == null) wordStart = i;
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

                    if ((leftFull || rightFull) && char.IsWhiteSpace(text[k]))
                        charStatus[k] = MatchStatus.Full;
                    else
                        charStatus[k] = MatchStatus.Partial;
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
                var span = LineMetadata.SpanRoots.FirstOrDefault(sr => i >= sr.RootToken.Match.RootMatch.Index && i < sr.RootToken.Match.RootMatch.Index + sr.RootToken.Match.RootMatch.Length);
                color = (span?.RootToken.Type == typeof(DefaultUnmatchedString)) ? "var(--unmatched-default)" : (span?.Palette.Normal ?? "var(--unmatched-default)");
            }

            rawSegments.Add(new TextStyledRun(text[i].ToString(), color, underlineClass));
        }

        TextRuns = CollapseSegments(rawSegments);
    }

    List<TextStyledRun> CollapseSegments(List<TextStyledRun> source)
    {
        if (source.Count == 0) return source;
        var result = new List<TextStyledRun>();
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

    string GetSuggestedClassName()
    {
        string str = "";
        foreach (var snippet in Snippets)
        {
            if (snippet is EditorPropertySnippet propertySnippet)
                str += propertySnippet.PropertyNameRepresentation;
            else if (snippet is EditorTextSnippet textSnippet)
            {
                var text = textSnippet.TrimmedText;
                if (string.IsNullOrWhiteSpace(text)) continue;
                var rawTextParts = text
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Regex.Replace(x, @"[^\w]+", ""))
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => char.ToUpper(x[0]) + x[1..].ToLower());
                str += string.Join("", rawTextParts);
            }
        }
        return string.IsNullOrEmpty(str) ? $"New{TokenUnitType.Name}" : str;
    }

    public void HandleActionOnSnippet(SnippetContextAction snippetContextAction)
    {
        var snippet = snippetContextAction.Snippet;
        var index = Snippets.IndexOf(snippet);

        switch (snippetContextAction.ActionType)
        {
            case ContextActionType.Delete: Snippets.Remove(snippet); break;

            case ContextActionType.RemoveOneOf:
            case ContextActionType.RemoveManyOf:
            case ContextActionType.RemoveCompoundOf: Snippets[index] = snippet.ConvertToXOfType(XOfType.None); break;

            case ContextActionType.ConvertToOneOf: Snippets[index] = snippet.ConvertToXOfType(XOfType.OneOf); break;
            case ContextActionType.ConvertToManyOf: Snippets[index] = snippet.ConvertToXOfType(XOfType.ManyOf); break;
            case ContextActionType.ConvertToCompoundOf: Snippets[index] = snippet.ConvertToXOfType(XOfType.CompoundOf); break;

            case ContextActionType.MakePlural:
            case ContextActionType.RemovePlural:
                Snippets[index] = snippet.UpdateProptions(oneHotToggleProptions: Proptions.Plural); break;

            case ContextActionType.MakeOptional:
            case ContextActionType.RemoveOptional:
                Snippets[index] = snippet.UpdateProptions(oneHotToggleProptions: Proptions.Optional); break;
        }

        Update();
    }

    string GetClassStringForSavingToFile() =>
        $$"""
        namespace {{SaveFileInNamespace}};

        public class {{ClassName}} : {{nameof(TokenUnit)}}
        {
            protected override Snippet[] Snippets => [{{string.Join(", ", _nonEmptySnippets.Select(x => x.ParameterRepresentation))}}];

            {{string.Join("\r\n    ", _nonEmptySnippets.OfType<EditorPropertySnippet>().Select(x => x.GetPropertyLineRepresentation()))}}
        }
        """;

    string GetClassStringForDisplayingHtml()
    {
        var classDeclaration = $"{Span("public class")} {Span(ClassName, SpanClass.type)} {Span(":", SpanClass.identifier)} {Span(TokenUnitType.Name, SpanClass.type)}";

        var snippetSection = $"{Span("protected override")} {Span("Snippet", SpanClass.type)}{Span("[]")} {Span("Snippets =>", SpanClass.identifier)} {Span("[", SpanClass.identifier)}"
            + string.Join(", ", _nonEmptySnippets.Select(x => x.GetParameterHtmlRepresentation()))
            + $"{Span("]", SpanClass.identifier)};";

        var propDeclarations = _nonEmptySnippets.OfType<EditorPropertySnippet>().Select(x => x.GetPropertyLineHtmlRepresentation());

        var styled = $$"""
            {{classDeclaration}}
            {
                {{snippetSection}}

                {{string.Join("\r\n    ", propDeclarations)}}
            }
            """;
        return styled.Replace("  ", "&nbsp;&nbsp;").Replace("\r\n", "<br>");
    }

    static string Span(string content, SpanClass spanClass = SpanClass.keyword) => $"<span class=\"{spanClass}\">{content}</span>";
}