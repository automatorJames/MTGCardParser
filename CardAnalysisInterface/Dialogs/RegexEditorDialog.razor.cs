using MTGPlexer.TokenAnalysisDTOs.SpanAnalysis;
using MTGPlexer.TokenUnitComponents;
using MTGPlexer.TokenUnits;

namespace CardAnalysisInterface.Dialogs;

public partial class RegexEditorDialog : ComponentBase, IAsyncDisposable
{
    [Parameter] public ProcessedLine Line { get; set; } = default!;
    [Parameter] public EventCallback<EditorTokenUnit> OnClose { get; set; }

    private string _className = "";
    private string ClassName
    {
        get => _className;
        set
        {
            if (_className == value) return;
            _className = Regex.Replace(value, @"\s+", "");
            _editorTokenUnit.Update(_currentRawPattern, _className);
        }
    }

    // Context Menu State
    private bool _isPillMenuVisible;
    private double _menuX;
    private double _menuY;
    private string _targetPillTypeName = "";
    private string _targetSnippetId = "";

    // Regex State
    private string _renderedRegex = "";
    private bool _classNameHasBeenManuallyEdited;
    private string _currentRawPattern = "";
    private List<RegexEditorSegment> _regexEditorSegments = new();
    private List<Match> _currentMatches = new();
    private EditorTokenUnit _editorTokenUnit = default!;

    // Autocomplete State
    private bool _isDropdownVisible = false;
    private bool _isEditingClassName = false;
    private bool _shouldFocusClassName = false;
    private List<Type> _allTokenTypes = new();
    private List<Type> _autocompleteSuggestions = new();
    private int _selectedSuggestionIndex = -1;
    private bool _isEditorEmpty = true;
    private string _textToReplaceForAutocomplete = "";

    // Interop
    private ElementReference _editorElement;
    private DotNetObjectReference<RegexEditorDialog>? _dotNetRef;

    public record RegexEditorSegment(string Text, string Color);
    private enum MatchStatus { None, Partial, Full }
    private record TextSegment(string Text, string Color, string UnderlineClass);

    protected override void OnInitialized()
    {
        _dotNetRef = DotNetObjectReference.Create(this);
        _editorTokenUnit = new EditorTokenUnit(Line);
        _allTokenTypes = TokenTypeRegistry.GetAllTypesExhaustive();
        _className = $"New{nameof(TokenUnit)}";
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && _dotNetRef != null)
        {
            var colorMap = _allTokenTypes.ToDictionary(
                t => t.Name,
                t => new {
                    Normal = DeterministicPalette.TypePaletteSet[t].Dark,
                    Highlight = DeterministicPalette.TypePaletteSet[t].Light
                }
            );

            await JsRuntime.InvokeVoidAsync("regexEditor.initialize", _dotNetRef, _editorElement, colorMap);
        }

        if (_isEditingClassName && _shouldFocusClassName)
        {
            _shouldFocusClassName = false;
            await JsRuntime.InvokeVoidAsync("regexEditor.focusClassNameInput", ".class-name-input");
        }
    }

    #region JS Invokable Methods

    [JSInvokable("OpenPillMenu")]
    public void OpenPillContextMenu(string typeName, string snippetId, double x, double y)
    {
        _targetPillTypeName = typeName;
        _targetSnippetId = snippetId;
        _menuX = x;
        _menuY = y;
        _isPillMenuVisible = true;
        StateHasChanged();
    }

    [JSInvokable("HideDropdown")]
    public void HideDropdown()
    {
        if (_isDropdownVisible)
        {
            _isDropdownVisible = false;
            StateHasChanged();
        }
    }

    [JSInvokable("HandleGlobalEscape")]
    public async Task HandleGlobalEscape()
    {
        if (_isPillMenuVisible)
        {
            _isPillMenuVisible = false;
            StateHasChanged();
            return;
        }

        if (_isDropdownVisible)
        {
            _isDropdownVisible = false;
            StateHasChanged();
        }
        else
        {
            await HandleCancel();
        }
    }

    [JSInvokable("NotifyContentChanged")]
    public async Task NotifyContentChanged(string rawText, string currentWord)
    {
        _isEditorEmpty = string.IsNullOrWhiteSpace(rawText);
        _currentRawPattern = rawText;

        if (!string.IsNullOrEmpty(currentWord) && currentWord.StartsWith("@"))
        {
            _textToReplaceForAutocomplete = currentWord;
            var filter = currentWord.Substring(1);

            _autocompleteSuggestions = _allTokenTypes
                .Where(t => t.Name.Contains(filter, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(t => t.Name.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
                .ThenBy(t => t.Name)
                .ToList();

            _isDropdownVisible = _autocompleteSuggestions.Any();
            _selectedSuggestionIndex = _isDropdownVisible ? 0 : -1;
        }
        else
        {
            _isDropdownVisible = false;
            _textToReplaceForAutocomplete = "";
        }

        UpdateRenderedRegexAndMatches(rawText);

        // If the dropdown is closed, ensure the HTML structure (pills) matches the raw text
        if (!_isDropdownVisible)
        {
            await SyncEditorPills();
        }

        StateHasChanged();
    }

    [JSInvokable("SelectSuggestionFromJS")]
    public async Task SelectSuggestionFromJS(string typeName)
    {
        var type = _allTokenTypes.FirstOrDefault(t => t.Name == typeName);
        if (type != null) await SelectSuggestionByKeyboard(type);
    }

    #endregion

    #region Editor Logic

    private async Task SyncEditorPills(int forceCaretPos = -1)
    {
        var metadata = _editorTokenUnit.EditorSnippets
            .Where(x => x.DisplayAsBlockInEditor)
            .Select(x => new { id = x.Id, typeName = x.EditorRepresentation })
            .ToList();

        // Get current caret to prevent jumping unless a position is forced
        var caretPos = forceCaretPos >= 0
            ? forceCaretPos
            : await JsRuntime.InvokeAsync<int>("regexEditor.getCaretOffset", _editorElement);

        await JsRuntime.InvokeVoidAsync("regexEditor.syncPills", _currentRawPattern, caretPos, metadata);
    }

    private async Task HandlePillDelete()
    {
        _isPillMenuVisible = false;
        _editorTokenUnit.RemoveSnippet(_targetSnippetId);
        _currentRawPattern = _editorTokenUnit.GetTemplateString();

        UpdateRenderedRegexAndMatches(_currentRawPattern);
        await SyncEditorPills();
        StateHasChanged();
    }

    private async Task SelectSuggestionByKeyboard(Type selection)
    {
        string fullTokenText = $"@{selection.Name}";
        // insertPill triggers the NotifyContentChanged flow via JS
        await JsRuntime.InvokeVoidAsync("regexEditor.insertPill", _textToReplaceForAutocomplete, fullTokenText);
        _isDropdownVisible = false;
    }

    private async Task OnKeyDown(KeyboardEventArgs e)
    {
        if (!_isDropdownVisible) return;

        switch (e.Key)
        {
            case "ArrowDown":
                _selectedSuggestionIndex = (_selectedSuggestionIndex + 1) % _autocompleteSuggestions.Count;
                await JsRuntime.InvokeVoidAsync("regexEditor.scrollToAutocompleteItem", $"autocomplete-item-{_selectedSuggestionIndex}");
                break;
            case "ArrowUp":
                _selectedSuggestionIndex = (_selectedSuggestionIndex - 1 + _autocompleteSuggestions.Count) % _autocompleteSuggestions.Count;
                await JsRuntime.InvokeVoidAsync("regexEditor.scrollToAutocompleteItem", $"autocomplete-item-{_selectedSuggestionIndex}");
                break;
            case "Enter":
            case "Tab":
                if (_selectedSuggestionIndex >= 0 && _selectedSuggestionIndex < _autocompleteSuggestions.Count)
                    await SelectSuggestionByKeyboard(_autocompleteSuggestions[_selectedSuggestionIndex]);
                break;
            case "Escape":
                _isDropdownVisible = false;
                break;
        }
    }

    #endregion

    #region Class Name Editing

    private void StartEditingClassName()
    {
        _isEditingClassName = true;
        _shouldFocusClassName = true;
        _classNameHasBeenManuallyEdited = true;
    }

    private void StopEditingClassName()
    {
        if (!_isEditingClassName) return;
        _isEditingClassName = false;
        UpdateRenderedRegexAndMatches(_currentRawPattern);
        StateHasChanged();
    }

    private void HandleClassNameKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter") StopEditingClassName();
        else if (e.Key == "Escape")
        {
            _isEditingClassName = false;
            StateHasChanged();
        }
    }

    #endregion

    #region Regex Rendering & Analysis

    private void UpdateRenderedRegexAndMatches(string patternToRender)
    {
        _currentMatches.Clear();
        var logicalPattern = patternToRender.Trim();

        if (string.IsNullOrWhiteSpace(logicalPattern))
        {
            _renderedRegex = "";
            _regexEditorSegments.Clear();
            _editorTokenUnit.Update(string.Empty);
            return;
        }

        try
        {
            var cleanPattern = logicalPattern.Replace('\u00A0', ' ');

            if (_classNameHasBeenManuallyEdited)
                _editorTokenUnit.Update(cleanPattern, ClassName);
            else
            {
                _editorTokenUnit.Update(cleanPattern);
                _className = _editorTokenUnit.ClassName;
            }

            _renderedRegex = _editorTokenUnit.RenderedRegex;
            ParseSegments();

            if (!string.IsNullOrWhiteSpace(_renderedRegex))
            {
                try
                {
                    _currentMatches = Regex.Matches(Line.SourceText.FormattedText, _renderedRegex)
                       .Cast<Match>()
                       .ToList();
                }
                catch (Exception ex)
                {
                    _renderedRegex = ex.Message;
                    _regexEditorSegments = new List<RegexEditorSegment> { new(_renderedRegex, "var(--error-red)") };
                }
            }
        }
        catch (Exception ex)
        {
            _renderedRegex = $"Error: {ex.Message}";
            _regexEditorSegments = new List<RegexEditorSegment> { new(_renderedRegex, "var(--error-red)") };
        }
    }

    private void ParseSegments()
    {
        _regexEditorSegments.Clear();
        if (string.IsNullOrEmpty(_renderedRegex)) return;

        int depth = 0;
        int lastPos = 0;

        for (int i = 0; i < _renderedRegex.Length; i++)
        {
            if (_renderedRegex[i] == '\\') { i++; continue; }

            if (_renderedRegex[i] == '(')
            {
                if (depth == 0)
                {
                    if (i > lastPos)
                        _regexEditorSegments.Add(new(_renderedRegex.Substring(lastPos, i - lastPos), "var(--syntax-default)"));
                    lastPos = i;
                }
                depth++;
            }
            else if (_renderedRegex[i] == ')')
            {
                depth--;
                if (depth == 0)
                {
                    var groupText = _renderedRegex.Substring(lastPos, i - lastPos + 1);
                    var match = Regex.Match(groupText, @"^\(\?<(?<name>[a-zA-Z0-9_]+)>");

                    string color = "var(--syntax-default)";
                    if (match.Success)
                    {
                        var name = match.Groups["name"].Value;
                        if (TokenTypeRegistry.NameToType.TryGetValue(name, out var type))
                            color = DeterministicPalette.TypePaletteSet[type].Normal;
                    }

                    _regexEditorSegments.Add(new(groupText, color));
                    lastPos = i + 1;
                }
            }
        }

        if (lastPos < _renderedRegex.Length)
            _regexEditorSegments.Add(new(_renderedRegex.Substring(lastPos), "var(--syntax-default)"));
    }

    private List<TextSegment> GetProcessedSegments()
    {
        var segments = new List<TextSegment>();
        string text = Line.SourceText.FormattedText;
        if (string.IsNullOrEmpty(text)) return segments;

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

        foreach (var m in _currentMatches)
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
                var span = Line.SpanRoots.FirstOrDefault(sr => i >= sr.RootToken.Match.RootMatch.Index && i < sr.RootToken.Match.RootMatch.Index + sr.RootToken.Match.RootMatch.Length);
                color = (span?.RootToken.Type == typeof(DefaultUnmatchedString)) ? "var(--unmatched-default)" : (span?.Palette.Normal ?? "var(--unmatched-default)");
            }
            segments.Add(new TextSegment(text[i].ToString(), color, underlineClass));
        }

        return CollapseSegments(segments);
    }

    private List<TextSegment> CollapseSegments(List<TextSegment> source)
    {
        if (!source.Any()) return source;
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

    #endregion

    private async Task SaveClassToFile()
    {
        if (_editorTokenUnit != null)
            TokenTypeRegistry.CreateAndRegisterNewTypeAndSaveToDisk(_editorTokenUnit);

        await OnClose.InvokeAsync(_editorTokenUnit);
    }

    private Task HandleCancel() => OnClose.InvokeAsync(null);

    public async ValueTask DisposeAsync()
    {
        if (_dotNetRef != null)
        {
            try { await JsRuntime.InvokeVoidAsync("regexEditor.dispose"); } catch { }
            _dotNetRef.Dispose();
        }
    }
}