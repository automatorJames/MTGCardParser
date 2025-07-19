namespace CardAnalysisInterface.Dialogs;

public partial class RegexEditorDialog : ComponentBase, IAsyncDisposable
{
    [Parameter]
    public CardLine Line { get; set; }

    [Parameter]
    public EventCallback<string> OnClose { get; set; }

    private string _renderedRegex = "";
    private List<Match> _currentMatches = new List<Match>();
    private DynamicTokenType _dynamicTokenType;

    private bool _isDropdownVisible = false;
    private List<Type> _allTemplateTypes = new();
    private List<Type> _autocompleteSuggestions = new();
    private int _selectedSuggestionIndex = -1;
    private bool _isEditorEmpty = true;
    private bool _showPreviewBoxes = false;

    private string _textToReplaceForAutocomplete = "";

    private ElementReference _editorElement;
    private DotNetObjectReference<RegexEditorDialog> _dotNetRef;

    protected override void OnInitialized()
    {
        _dotNetRef = DotNetObjectReference.Create(this);
        _allTemplateTypes.AddRange(TokenTypeRegistry.AppliedOrderTypes);
        _allTemplateTypes.AddRange(TokenTypeRegistry.ReferencedEnumTypes);
        _allTemplateTypes = _allTemplateTypes.OrderBy(t => t.Name).ToList();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JsRuntime.InvokeVoidAsync("initializeEditor", _dotNetRef, _editorElement);
        }
    }

    [JSInvokable]
    public void HideDropdown()
    {
        _isDropdownVisible = false;
        StateHasChanged();
    }

    [JSInvokable]
    public Task HandleGlobalEscape()
    {
        if (_isDropdownVisible)
        {
            _isDropdownVisible = false;
            StateHasChanged();
            return Task.CompletedTask;
        }
        else
        {
            return HandleCancel();
        }
    }

    [JSInvokable]
    public void UpdateFromJavaScript(string rawText, string currentWord)
    {
        _isEditorEmpty = string.IsNullOrEmpty(rawText);

        if (currentWord.StartsWith("@"))
        {
            _textToReplaceForAutocomplete = currentWord;
            var filter = currentWord.Substring(1);
            _autocompleteSuggestions = _allTemplateTypes
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
        StateHasChanged();
    }

    private async Task OnKeyDown(KeyboardEventArgs e)
    {
        if (!_isDropdownVisible)
        {
            return;
        }

        switch (e.Key)
        {
            case "ArrowDown":
                _selectedSuggestionIndex = (_selectedSuggestionIndex + 1) % _autocompleteSuggestions.Count;
                StateHasChanged(); // This is needed to update the 'selected' class
                await JsRuntime.InvokeVoidAsync("scrollToAutocompleteItem", $"autocomplete-item-{_selectedSuggestionIndex}");
                break;
            case "ArrowUp":
                _selectedSuggestionIndex = (_selectedSuggestionIndex - 1 + _autocompleteSuggestions.Count) % _autocompleteSuggestions.Count;
                StateHasChanged(); // This is needed to update the 'selected' class
                await JsRuntime.InvokeVoidAsync("scrollToAutocompleteItem", $"autocomplete-item-{_selectedSuggestionIndex}");
                break;
            case "Enter":
            case "Tab":
                if (_selectedSuggestionIndex != -1 && _autocompleteSuggestions.Count > _selectedSuggestionIndex)
                {
                    await SelectSuggestionByKeyboard(_autocompleteSuggestions[_selectedSuggestionIndex]);
                }
                break;
        }
    }

    private async Task SelectSuggestionByKeyboard(Type selection)
    {
        string fullTokenText = $"@{selection.Name}";
        await JsRuntime.InvokeVoidAsync("commitToken", _textToReplaceForAutocomplete, fullTokenText);
        _isDropdownVisible = false;
    }

    private void UpdateRenderedRegexAndMatches(string patternToRender)
    {
        var logicalPattern = patternToRender.Trim();

        _showPreviewBoxes = logicalPattern.Contains("@");

        if (!_showPreviewBoxes)
        {
            _renderedRegex = logicalPattern;
            _dynamicTokenType = null;
        }

        try
        {
            if (_showPreviewBoxes)
            {
                _dynamicTokenType = new DynamicTokenType(logicalPattern);
                _renderedRegex = _dynamicTokenType.RenderedRegex;
            }

            _currentMatches = string.IsNullOrWhiteSpace(_renderedRegex)
                ? new List<Match>()
                : Regex.Matches(Line.SourceText, _renderedRegex).Cast<Match>().ToList();
        }
        catch (Exception)
        {
            _renderedRegex = "Error: Invalid template";
            _currentMatches.Clear();
        }
    }

    private async Task HandleSubmit()
    {
        TokenTypeRegistry.AddNewTypeAndSaveToDisk(_dynamicTokenType);
        await OnClose.InvokeAsync();
    }

    private Task HandleCancel() => OnClose.InvokeAsync(null);

    public async ValueTask DisposeAsync()
    {
        if (_dotNetRef != null)
        {
            await JsRuntime.InvokeVoidAsync("disposeEditor");
            _dotNetRef.Dispose();
        }
    }
}