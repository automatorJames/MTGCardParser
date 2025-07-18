namespace CardAnalysisInterface.Dialogs;

public partial class RegexEditorDialog : ComponentBase
{

    [Parameter]
    public CardLine Line { get; set; }

    [Parameter]
    public EventCallback<string> OnClose { get; set; }


    List<RegexPatternDisplaySegment> _patternSegments = new();
    string _currentTypingText = "";
    string _renderedRegex = "";
    List<Match> _currentMatches = new List<Match>();
    TokenTemplatePreview _tokenTemplatePreview;

    bool _isDropdownVisible = false;
    List<Type> _allTemplateTypes = new();
    List<Type> _autocompleteSuggestions = new();
    int _selectedSuggestionIndex = -1;
    ElementReference _inputElement;

    // Computed property for the full regex string
    private string _newRegexPattern => string.Concat(_patternSegments.Select(s => s.OriginalText));

    protected override void OnInitialized()
    {
        _allTemplateTypes.AddRange(TokenTypeRegistry.AppliedOrderTypes);
        _allTemplateTypes.AddRange(TokenTypeRegistry.ReferencedEnumTypes);
        _allTemplateTypes = _allTemplateTypes.OrderBy(t => t.Name).ToList();
        UpdateAndRender();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JsRuntime.InvokeVoidAsync("initializeAutocompleteInteraction", "autocomplete-dropdown-list");
            await JsRuntime.InvokeVoidAsync("registerDialogKeyListener", DotNetObjectReference.Create(this));
            await JsRuntime.InvokeVoidAsync("focusElement", "regex-ghost-input");
        }
    }

    private void OnTextInput(ChangeEventArgs e)
    {
        JsRuntime.InvokeVoidAsync("setKeyboardNavigating", false);
        _currentTypingText = e.Value?.ToString() ?? "";
        var fullPatternForSuggestions = _newRegexPattern + _currentTypingText;

        int atIndex = _currentTypingText.LastIndexOf('@');
        if (atIndex != -1 && !_currentTypingText.Substring(atIndex).Contains(' '))
        {
            var filter = _currentTypingText.Substring(atIndex + 1);
            _autocompleteSuggestions = _allTemplateTypes
                .Where(t => t.Name.Contains(filter, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(t => t.Name.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
                .ThenBy(t => t.Name)
                .ToList();

            _isDropdownVisible = _autocompleteSuggestions.Any();
            _selectedSuggestionIndex = -1;
        }
        else
        {
            _isDropdownVisible = false;
        }
        UpdateRenderedRegexAndMatches();
    }

    private void CommitTypingText()
    {
        if (!string.IsNullOrEmpty(_currentTypingText))
        {
            // If the text being typed contains a pending autocomplete, don't commit it.
            int atIndex = _currentTypingText.LastIndexOf('@');
            if (atIndex == -1 || _currentTypingText.Substring(atIndex).Contains(' '))
            {
                _patternSegments.Add(new RegexPatternDisplaySegment(_currentTypingText, IsPill: false));
                _currentTypingText = "";
                UpdateAndRender();
            }
        }
    }

    private async Task OnInputKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Backspace" && string.IsNullOrEmpty(_currentTypingText) && _patternSegments.Any())
        {
            var lastSegment = _patternSegments.Last();
            if (lastSegment.IsPill)
            {
                _patternSegments.RemoveAt(_patternSegments.Count - 1);
            }
            else // It's a text segment, move it to the typing area for editing.
            {
                _currentTypingText = lastSegment.OriginalText;
                _patternSegments.RemoveAt(_patternSegments.Count - 1);
            }
            UpdateAndRender();
            return;
        }

        if (!_isDropdownVisible) return;

        switch (e.Key)
        {
            case "ArrowDown":
            case "ArrowUp":
                await JsRuntime.InvokeVoidAsync("setKeyboardNavigating", true);
                if (e.Key == "ArrowDown")
                    _selectedSuggestionIndex = (_selectedSuggestionIndex + 1) % _autocompleteSuggestions.Count;
                else
                    _selectedSuggestionIndex = (_selectedSuggestionIndex - 1 + _autocompleteSuggestions.Count) % _autocompleteSuggestions.Count;
                await JsRuntime.InvokeVoidAsync("scrollToElement", $"autocomplete-item-{_selectedSuggestionIndex}");
                break;
            case "Enter" or "Tab":
                if (_selectedSuggestionIndex != -1)
                {
                    await SelectSuggestion(_autocompleteSuggestions[_selectedSuggestionIndex]);
                }
                break;
            case "Escape":
                _isDropdownVisible = false;
                break;
        }
    }

    private async Task SelectSuggestion(Type selection)
    {
        int atIndex = _currentTypingText.LastIndexOf('@');
        string pretext = _currentTypingText.Substring(0, atIndex);
        if (!string.IsNullOrEmpty(pretext))
        {
            _patternSegments.Add(new RegexPatternDisplaySegment(pretext));
        }

        var pillText = $"@{selection.Name}";
        //_patternSegments.Add(new RegexPatternDisplaySegment { OriginalText = pillText, DisplayText = selection.Name, IsPill = true });
        _patternSegments.Add(new RegexPatternDisplaySegment(pillText, IsPill: true));

        _currentTypingText = " "; // Add a space for better UX
        _isDropdownVisible = false;
        await JsRuntime.InvokeVoidAsync("setKeyboardNavigating", false);
        UpdateAndRender();
        await FocusInputElement();
    }

    private void DeletePill(Guid segmentId)
    {
        _patternSegments.RemoveAll(s => s.Id == segmentId);
        UpdateAndRender();
        FocusInputElement();
    }

    private void UpdateAndRender()
    {
        UpdateRenderedRegexAndMatches();
        StateHasChanged();
    }

    private void UpdateRenderedRegexAndMatches()
    {
        var patternToRender = _newRegexPattern + _currentTypingText;
        if (!patternToRender.Contains("@"))
        {
            _renderedRegex = "";
            try
            {
                if (!string.IsNullOrWhiteSpace(patternToRender))
                    _currentMatches = Regex.Matches(Line.SourceText, patternToRender).Cast<Match>().ToList();
                else
                    _currentMatches.Clear();
            }
            catch (Exception) { _currentMatches.Clear(); }
            return;
        }

        try
        {
            _tokenTemplatePreview = new(patternToRender);
            _renderedRegex = _tokenTemplatePreview.RenderedRegex;
            _currentMatches = Regex.Matches(Line.SourceText, _renderedRegex).Cast<Match>().ToList();
        }
        catch (Exception)
        {
            _renderedRegex = "Error: Invalid template";
            _currentMatches.Clear();
        }
    }

    private async Task HandleSubmit()
    {
        CommitTypingText();
        await OnClose.InvokeAsync(_newRegexPattern);
    }

    private Task HandleCancel() => OnClose.InvokeAsync(null);
    private async Task FocusInputElement() => await _inputElement.FocusAsync();

    [JSInvokable]
    public Task HandleEscapeKeyPress() => HandleCancel();

    public async ValueTask DisposeAsync()
    {
        await JsRuntime.InvokeVoidAsync("disposeDialogKeyListener");
    }
}