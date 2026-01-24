using MTGPlexer.TokenAnalysisDTOs.SpanAnalysis;

namespace CardAnalysisInterface.Dialogs;

public partial class RegexEditorDialog : ComponentBase, IAsyncDisposable
{
    [Parameter] public ProcessedLine Line { get; set; } = default!;
    [Parameter] public EventCallback<EditorTokenUnit> OnClose { get; set; }

    private string ClassName
    {
        get
        {
            return _editorTokenUnit.ClassName;
        }
        set
        {
            if (_editorTokenUnit.ClassName == value)
                return;

            string cleanName = Regex.Replace(value, @"\s+", "");
            _editorTokenUnit.Update(preferredClassName: cleanName);
        }
    }

    // Context Menu State
    private bool _isPillMenuVisible;
    private double _menuX;
    private double _menuY;
    private string _targetPillTypeName = "";
    private string _targetSnippetId = "";

    private readonly List<ContextAction> _pillContextMenuOptions = new()
    {
        new(ContextActionType.ConvertToOneOf),
        new(ContextActionType.ConvertToManyOf),
        new(ContextActionType.ConvertToCompoundOf),
        new(ContextActionType.Delete, sectionBreak: true),
    };

    // UI State
    private bool _isDropdownVisible = false;
    private bool _isEditingClassName = false;
    private bool _shouldFocusClassName = false;
    private List<Type> _allTokenTypes = new();
    private List<Type> _autocompleteSuggestions = new();
    private int _selectedSuggestionIndex = -1;
    private bool _isEditorEmpty = true;
    private string _textToReplaceForAutocomplete = "";

    // Domain Model
    private EditorTokenUnit _editorTokenUnit = default!;

    // Interop
    private ElementReference _editorElement;
    private DotNetObjectReference<RegexEditorDialog> _dotNetRef;

    protected override void OnInitialized()
    {
        _dotNetRef = DotNetObjectReference.Create(this);
        _editorTokenUnit = new EditorTokenUnit(Line);
        _allTokenTypes = TokenTypeRegistry.GetAllTypesExhaustive();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && _dotNetRef != null)
        {
            // Requirement 2: Only send the base color to JS
            var colorMap = _allTokenTypes.ToDictionary(
                t => t.Name,
                t => DeterministicPalette.TypePaletteSet[t].Dark
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
        var propertySnippet = _editorTokenUnit[snippetId];
        _targetPillTypeName = propertySnippet?.GetContextMenuDisplayName() ?? typeName;
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
    public async Task NotifyContentChanged(string rawText, string currentWord, int forceCaretPos)
    {
        _isEditorEmpty = string.IsNullOrWhiteSpace(rawText);

        _editorTokenUnit.Update(templateString: rawText);

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

        if (!_isDropdownVisible)
            await SyncEditorPills(forceCaretPos);

        StateHasChanged();
    }

    [JSInvokable("SelectSuggestionFromJS")]
    public async Task SelectSuggestionFromJS(string typeName)
    {
        var type = _allTokenTypes.FirstOrDefault(t => t.Name == typeName);
        if (type != null)
        {
            await SelectSuggestionByKeyboard(type);
        }
    }

    #endregion

    #region UI Logic

    private async Task SyncEditorPills(int forceCaretPos = -1)
    {
        var metadata = _editorTokenUnit.Snippets
            .Where(x => x.DisplayAsBlockInEditor)
            .Select(x => new { id = x.Id, typeName = x.EditorRepresentation })
            .ToList();

        var caretPos = forceCaretPos >= 0
            ? forceCaretPos
            : await JsRuntime.InvokeAsync<int>("regexEditor.getCaretOffset", _editorElement);

        await JsRuntime.InvokeVoidAsync("regexEditor.syncPills", _editorTokenUnit.RawTemplate, caretPos, metadata);
    }

    private void CloseContextMenu()
    {
        _isPillMenuVisible = false;
        StateHasChanged();
    }

    private async Task HandlePillAction(ContextAction action)
    {
        _isPillMenuVisible = false;

        if (action.Type == ContextActionType.Delete)
        {
            _editorTokenUnit.RemoveSnippet(_targetSnippetId);
            await SyncEditorPills();
        }

        StateHasChanged();
    }

    private async Task SelectSuggestionByKeyboard(Type selection)
    {
        string fullTokenText = $"@{selection.Name}";
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
                {
                    await SelectSuggestionByKeyboard(_autocompleteSuggestions[_selectedSuggestionIndex]);
                }
                break;
            case "Escape":
                _isDropdownVisible = false;
                break;
        }
    }

    private void StartEditingClassName()
    {
        _isEditingClassName = true;
        _shouldFocusClassName = true;
    }

    private void StopEditingClassName()
    {
        _isEditingClassName = false;
        StateHasChanged();
    }

    private void HandleClassNameKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            StopEditingClassName();
        }
        else if (e.Key == "Escape")
        {
            _isEditingClassName = false;
            StateHasChanged();
        }
    }

    #endregion

    private async Task SaveClassToFile()
    {
        TokenTypeRegistry.CreateAndRegisterNewTypeAndSaveToDisk(_editorTokenUnit);
        await OnClose.InvokeAsync(_editorTokenUnit);
    }

    private Task HandleCancel()
    {
        return OnClose.InvokeAsync(null);
    }

    public async ValueTask DisposeAsync()
    {
        if (_dotNetRef != null)
        {
            try
            {
                await JsRuntime.InvokeVoidAsync("regexEditor.dispose");
            }
            catch { }

            _dotNetRef.Dispose();
        }
    }
}