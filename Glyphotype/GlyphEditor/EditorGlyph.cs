namespace Glyphotype.GlyphEditor;

public class EditorGlyph
{
    const string SaveFileInNamespace = "MTGGlyphs";

    public ProcessedLine LineMetadata { get; }
    public Type GlyphType { get; } = typeof(Glyph);
    public List<EditorNib> Nibs { get; private set; } = [];

    string _suggestedClassName = "";
    public string ManualClassName { get; private set; }
    public string ClassName => ManualClassName ?? _suggestedClassName;

    public string RawTemplate => string.Concat(Nibs.Select(s => s.EditorRepresentation));
    public string RenderedRegex { get; private set; } = "";
    public string ClassStringForSavingToFile { get; private set; } = "";
    public string ClassStringForDisplayingHtml { get; private set; } = "";
    public List<RegexStyledRun> RegexRuns { get; private set; } = [];
    public List<TextStyledRun> TextRuns { get; private set; } = [];
    public List<Match> CurrentMatches { get; private set; } = [];

    IEnumerable<EditorNib> _nonEmptyNibs =>
        Nibs.Where(x => x is not EditorTextNib textNib || !string.IsNullOrWhiteSpace(textNib.RawText));

    public EditorGlyph(ProcessedLine lineMetadata)
    {
        LineMetadata = lineMetadata;
        _suggestedClassName = $"New{GlyphType.Name}";
        Update([]);
    }

    public EditorPropertyNib this[string id] =>
        Nibs.OfType<EditorPropertyNib>().FirstOrDefault(x => x.Id == id);

    public void Update(List<TemplateFragment> fragments = null, string preferredClassName = null)
    {
        //if (fragments != null)
        //{
        //    SyncNibsFromFragments(fragments);
        //    _suggestedClassName = GetSuggestedClassName();
        //}
        //
        //if (preferredClassName != null)
        //    ManualClassName = string.IsNullOrWhiteSpace(preferredClassName) ? null : preferredClassName;
        //
        //RenderedRegex = CompositionFactory.GetComposedString(_nonEmptyNibs.Select(x => x.ToNamedGroupNode()), GlyphType);
        //
        //ParseRegexSegments();
        //PerformMatching();
        //GenerateTextStyledRuns();
        //
        //ClassStringForSavingToFile = GetClassStringForSavingToFile();
        //ClassStringForDisplayingHtml = GetClassStringForDisplayingHtml();
    }

    private void SyncNibsFromFragments(List<TemplateFragment> fragments)
    {
        var newNibs = new List<EditorNib>();
        var existingNibMap = Nibs.Where(s => s.Id != null).ToDictionary(s => s.Id);
        int autoIdCounter = 0;

        foreach (var frag in fragments)
        {
            if (frag.IsPill)
            {
                // If we already have this nib in our internal list, keep it.
                // This preserves its XOfType, Proptions, and other internal states.
                if (frag.Id != null && existingNibMap.TryGetValue(frag.Id, out var existing))
                {
                    newNibs.Add(existing);
                }
                else
                {
                    // Brand new pill (e.g. just inserted from autocomplete)
                    var newNib = CreateNibFromFragment(frag);
                    if (newNib != null) newNibs.Add(newNib);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(frag.Text)) continue;
                newNibs.Add(new EditorTextNib(frag.Text, $"txt-{autoIdCounter++}"));
            }
        }

        Nibs = CollapseTextNibs(newNibs);
    }

    private EditorNib CreateNibFromFragment(TemplateFragment frag)
    {
        if (!frag.IsPill) return new EditorTextNib(frag.Text, frag.Id);

        if (!string.IsNullOrEmpty(frag.MethodName))
            if (Enum.TryParse<ShortcutNibMethod>(frag.MethodName, out var methodType))
                return new EditorMethodNib(methodType, frag.Args ?? [], frag.Id);

        if (!string.IsNullOrEmpty(frag.TypeName))
            if (GlyphTypeRegistry.NameToType.TryGetValue(frag.TypeName, out Type parsedBaseType))
                return new EditorPropertyNib(parsedBaseType, XOfType.None, frag.Id);

        return new EditorTextNib(frag.Text, frag.Id);
    }

    private List<EditorNib> CollapseTextNibs(List<EditorNib> source)
    {
        if (source.Count == 0) return source;

        var result = new List<EditorNib>();
        EditorTextNib currentText = null;

        foreach (var nib in source)
        {
            if (nib is EditorTextNib textNib)
            {
                if (currentText == null) currentText = textNib;
                else currentText = currentText with { RawText = currentText.RawText + textNib.RawText };
            }
            else
            {
                if (currentText != null) { result.Add(currentText); currentText = null; }
                result.Add(nib);
            }
        }

        if (currentText != null) result.Add(currentText);
        return result;
    }

    public List<TemplateFragment> GetTemplateFragments()
    {
        return Nibs.Select(s => {
            string typeName = (s as EditorPropertyNib)?.BasePropertyType.Name;
            string methodName = (s as EditorMethodNib)?.MethodType.ToString();
            string[] args = (s as EditorMethodNib)?.Args;

            return new TemplateFragment(
                Text: s.EditorRepresentation,
                Id: s.Id,
                IsPill: s is EditorBlockNib,
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
                        if (GlyphTypeRegistry.NameToType.TryGetValue(name, out var type))
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
        //var rawSegments = new List<TextStyledRun>();
        //string text = LineMetadata.SourceText.FormattedText;
        //
        //if (string.IsNullOrEmpty(text)) { TextRuns = []; return; }
        //
        //var charStatus = new MatchStatus[text.Length];
        //Array.Fill(charStatus, MatchStatus.None);
        //
        //var words = new List<(int Start, int End)>();
        //int? wordStart = null;
        //
        //for (int i = 0; i <= text.Length; i++)
        //{
        //    bool isWordChar = i < text.Length && !char.IsWhiteSpace(text[i]);
        //    if (isWordChar && wordStart == null) wordStart = i;
        //    else if (!isWordChar && wordStart != null)
        //    {
        //        words.Add((wordStart.Value, i - 1));
        //        wordStart = null;
        //    }
        //}
        //
        //foreach (var m in CurrentMatches)
        //{
        //    int mStart = m.Index;
        //    int mEnd = m.Index + m.Length - 1;
        //    var overlappingWords = words.Where(w => mStart <= w.End && mEnd >= w.Start);
        //
        //    foreach (var word in overlappingWords)
        //    {
        //        int strippedEnd = (text[word.End] == '.') ? word.End - 1 : word.End;
        //        bool coversFull = (mStart <= word.Start && mEnd >= word.End);
        //        bool coversStripped = (mStart <= word.Start && mEnd == strippedEnd && strippedEnd < word.End);
        //
        //        if (coversFull || coversStripped)
        //            for (int k = Math.Max(mStart, word.Start); k <= Math.Min(mEnd, word.End); k++)
        //                charStatus[k] = MatchStatus.Full;
        //        else
        //            for (int k = Math.Max(mStart, word.Start); k <= Math.Min(mEnd, word.End); k++)
        //                if (charStatus[k] == MatchStatus.None) charStatus[k] = MatchStatus.Partial;
        //    }
        //
        //    for (int k = mStart; k <= mEnd; k++)
        //    {
        //        if (charStatus[k] == MatchStatus.None)
        //        {
        //            bool leftFull = k > 0 && charStatus[k - 1] == MatchStatus.Full;
        //            bool rightFull = k < text.Length - 1 && charStatus[k + 1] == MatchStatus.Full;
        //
        //            if ((leftFull || rightFull) && char.IsWhiteSpace(text[k]))
        //                charStatus[k] = MatchStatus.Full;
        //            else
        //                charStatus[k] = MatchStatus.Partial;
        //        }
        //    }
        //}
        //
        //for (int i = 0; i < text.Length; i++)
        //{
        //    string color;
        //    string underlineClass = "";
        //    MatchStatus status = charStatus[i];
        //
        //    if (status == MatchStatus.Full)
        //    {
        //        color = "var(--match-full-text)";
        //        underlineClass = "full-match";
        //    }
        //    else if (status == MatchStatus.Partial)
        //    {
        //        color = "var(--match-partial-text)";
        //        underlineClass = "partial-match";
        //    }
        //    else
        //    {
        //        var span = LineMetadata.SpanRoots.FirstOrDefault(sr => i >= sr.RootToken.Match.RootMatch.Index && i < sr.RootToken.Match.RootMatch.Index + //sr.RootToken.Match.RootMatch.Length);
        //        color = (span?.RootToken.Type == typeof(UnmatchedString)) ? "var(--unmatched-default)" : (span?.Palette.Normal ?? "var(--unmatched-default)");
        //    }
        //
        //    rawSegments.Add(new TextStyledRun(text[i].ToString(), color, underlineClass));
        //}
        //
        //TextRuns = CollapseSegments(rawSegments);
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
        foreach (var nib in Nibs)
        {
            if (nib is EditorPropertyNib propertyNib)
                str += propertyNib.PropertyNameRepresentation;
            else if (nib is EditorTextNib textNib)
            {
                var text = textNib.TrimmedText;
                if (string.IsNullOrWhiteSpace(text)) continue;
                var rawTextParts = text
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Regex.Replace(x, @"[^\w]+", ""))
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => char.ToUpper(x[0]) + x[1..].ToLower());
                str += string.Join("", rawTextParts);
            }
        }
        return string.IsNullOrEmpty(str) ? $"New{GlyphType.Name}" : str;
    }

    public void HandleActionOnNib(NibContextAction nibContextAction)
    {
        var nib = nibContextAction.Nib;
        var index = Nibs.IndexOf(nib);

        switch (nibContextAction.ActionType)
        {
            case ContextActionType.Delete: Nibs.Remove(nib); break;

            case ContextActionType.RemoveOneOf:
            case ContextActionType.RemoveManyOf:
            case ContextActionType.RemoveCompoundOf: Nibs[index] = nib.ConvertToXOfType(XOfType.None); break;

            case ContextActionType.ConvertToOneOf: Nibs[index] = nib.ConvertToXOfType(XOfType.OneOf); break;
            case ContextActionType.ConvertToManyOf: Nibs[index] = nib.ConvertToXOfType(XOfType.ManyOf); break;
            case ContextActionType.ConvertToCompoundOf: Nibs[index] = nib.ConvertToXOfType(XOfType.CompoundOf); break;

            case ContextActionType.MakePlural:
            case ContextActionType.RemovePlural:
                Nibs[index] = nib.UpdateProptions(oneHotToggleProptions: Proptions.Plural); break;

            case ContextActionType.MakeOptional:
            case ContextActionType.RemoveOptional:
                Nibs[index] = nib.UpdateProptions(oneHotToggleProptions: Proptions.Optional); break;
        }

        Update();
    }

    string GetClassStringForSavingToFile() =>
        $$"""
        namespace {{SaveFileInNamespace}};

        public class {{ClassName}} : {{nameof(Glyph)}}
        {
            public override Nib[] Nibs => [{{string.Join(", ", _nonEmptyNibs.Select(x => x.ParameterRepresentation))}}];

            {{string.Join("\r\n    ", _nonEmptyNibs.OfType<EditorPropertyNib>().Select(x => x.GetPropertyLineRepresentation()))}}
        }
        """;

    string GetClassStringForDisplayingHtml()
    {
        var classDeclaration = $"{Span("public class")} {Span(ClassName, SpanClass.type)} {Span(":", SpanClass.identifier)} {Span(GlyphType.Name, SpanClass.type)}";

        var nibSection = $"{Span("protected override")} {Span("Nib", SpanClass.type)}{Span("[]")} {Span("Nibs =>", SpanClass.identifier)} {Span("[", SpanClass.identifier)}"
            + string.Join(", ", _nonEmptyNibs.Select(x => x.GetParameterHtmlRepresentation()))
            + $"{Span("]", SpanClass.identifier)};";

        var propDeclarations = _nonEmptyNibs.OfType<EditorPropertyNib>().Select(x => x.GetPropertyLineHtmlRepresentation());

        var styled = $$"""
            {{classDeclaration}}
            {
                {{nibSection}}

                {{string.Join("\r\n    ", propDeclarations)}}
            }
            """;
        return styled.Replace("  ", "&nbsp;&nbsp;").Replace("\r\n", "<br>");
    }

    static string Span(string content, SpanClass spanClass = SpanClass.keyword) => $"<span class=\"{spanClass}\">{content}</span>";
}