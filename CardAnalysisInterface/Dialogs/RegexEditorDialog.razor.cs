namespace CardAnalysisInterface.Dialogs;

public partial class RegexEditorDialog : ComponentBase, IAsyncDisposable
{
    [Parameter] public ProcessedLine Line { get; set; } = default!;
    [Parameter] public EventCallback<EditorTokenUnit> OnClose { get; set; }

    string ClassName
    {
        get => _editorTokenUnit.ClassName;
        set
        {
            if (_editorTokenUnit.ClassName == value)
                return;

            string cleanName = Regex.Replace(value, @"\s+", "");
            _editorTokenUnit.Update(preferredClassName: cleanName);
        }
    }

    // Context Menu State
    bool _isPillMenuVisible;
    double _menuX;
    double _menuY;
    string _targetPillTypeName = "";
    EditorPropertySnippet _targetPropertySnippet;

    // UI State
    bool _isDropdownVisible = false;
    bool _isEditingClassName = false;
    bool _shouldFocusClassName = false;
    List<Type> _allTokenTypes = new();
    List<Type> _autocompleteSuggestions = new();
    int _selectedSuggestionIndex = -1;
    bool _isEditorEmpty = true;
    string _textToReplaceForAutocomplete = "";

    // Domain Model
    EditorTokenUnit _editorTokenUnit = default!;

    // Interop
    ElementReference _editorElement;
    DotNetObjectReference<RegexEditorDialog> _dotNetRef;

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
            var colorMap = _allTokenTypes.ToDictionary(
                t => t.Name,
                t => DeterministicPalette.TypePaletteSet[t].Dark
            );

            await JsRuntime.InvokeVoidAsync("regexEditor.initialize", _dotNetRef, _editorElement, colorMap);
            await SyncEditorPills();
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
        _targetPropertySnippet = _editorTokenUnit[snippetId];
        if (_targetPropertySnippet == null) return;

        _targetPillTypeName = _targetPropertySnippet.GetContextMenuDisplayName() ?? typeName;
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
        if (_isPillMenuVisible) { _isPillMenuVisible = false; StateHasChanged(); return; }

        if (_isDropdownVisible) { _isDropdownVisible = false; StateHasChanged(); }
        else await HandleCancel();
    }

    [JSInvokable("NotifyContentChanged")]
    public async Task NotifyContentChanged(List<TemplateFragment> fragments, string currentWord, int forceCaretPos)
    {
        _isEditorEmpty = fragments.Count == 0 || (fragments.Count == 1 && string.IsNullOrWhiteSpace(fragments[0].Text));

        _editorTokenUnit.Update(fragments: fragments);

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

        // Always sync back to ensure JS and C# are 100% aligned on pill IDs and text concatenation
        await SyncEditorPills(forceCaretPos);
        StateHasChanged();
    }

    [JSInvokable("SelectSuggestionFromJS")]
    public async Task SelectSuggestionFromJS(string typeName)
    {
        var type = _allTokenTypes.FirstOrDefault(t => t.Name == typeName);
        if (type != null) await SelectSuggestionByKeyboard(type);
    }

    #endregion

    #region UI Logic

    private async Task SyncEditorPills(int forceCaretPos = -1)
    {
        var fragments = _editorTokenUnit.GetTemplateFragments();
        var caretPos = forceCaretPos >= 0
            ? forceCaretPos
            : await JsRuntime.InvokeAsync<int>("regexEditor.getCaretOffset");

        await JsRuntime.InvokeVoidAsync("regexEditor.syncPills", fragments, caretPos);
    }

    private void CloseContextMenu()
    {
        _isPillMenuVisible = false;
        StateHasChanged();
    }

    private async Task HandlePillAction(SnippetContextAction action)
    {
        _isPillMenuVisible = false;
        _editorTokenUnit.HandleActionOnSnippet(action);
        await SyncEditorPills();
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
                    await SelectSuggestionByKeyboard(_autocompleteSuggestions[_selectedSuggestionIndex]);
                break;
            case "Escape":
                _isDropdownVisible = false;
                break;
        }
    }

    private void StartEditingClassName() { _isEditingClassName = true; _shouldFocusClassName = true; }
    private void StopEditingClassName() { _isEditingClassName = false; StateHasChanged(); }

    private void HandleClassNameKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter") StopEditingClassName();
        else if (e.Key == "Escape") { _isEditingClassName = false; StateHasChanged(); }
    }

    #endregion

    private async Task SaveClassToFile()
    {
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