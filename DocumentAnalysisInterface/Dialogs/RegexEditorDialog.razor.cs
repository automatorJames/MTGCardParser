namespace DocumentAnalysisInterface.Dialogs;

public partial class RegexEditorDialog : ComponentBase, IAsyncDisposable
{
    [Parameter] public ProcessedLine Line { get; set; } = default!;
    [Parameter] public EventCallback<EditorGlyph> OnClose { get; set; }

    string ClassName
    {
        get => _editorGlyph.ClassName;
        set
        {
            if (_editorGlyph.ClassName == value)
                return;

            string cleanName = Regex.Replace(value, @"\s+", "");
            _editorGlyph.Update(preferredClassName: cleanName);
        }
    }

    // Context Menu State
    bool _isPillMenuVisible;
    double _menuX;
    double _menuY;
    string _targetPillTypeName = "";
    EditorPropertyNib _targetPropertyNib;

    // UI State
    bool _isDropdownVisible = false;
    bool _isEditingClassName = false;
    bool _shouldFocusClassName = false;
    List<Type> _allGlyphTypes = new();
    List<Type> _autocompleteSuggestions = new();
    int _selectedSuggestionIndex = -1;
    bool _isEditorEmpty = true;
    string _textToReplaceForAutocomplete = "";

    // Domain Model
    EditorGlyph _editorGlyph = default!;

    // Interop
    ElementReference _editorElement;
    DotNetObjectReference<RegexEditorDialog> _dotNetRef;

    protected override void OnInitialized()
    {
        _dotNetRef = DotNetObjectReference.Create(this);
        _editorGlyph = new EditorGlyph(Line);
        _allGlyphTypes = GlyphTypeRegistry.GetAllTypesExhaustive();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && _dotNetRef != null)
        {
            var colorMap = _allGlyphTypes.ToDictionary(
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
    public void OpenPillContextMenu(string typeName, string nibId, double x, double y)
    {
        _targetPropertyNib = _editorGlyph[nibId];
        if (_targetPropertyNib == null) return;

        _targetPillTypeName = _targetPropertyNib.GetContextMenuDisplayName() ?? typeName;
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

        _editorGlyph.Update(fragments: fragments);

        if (!string.IsNullOrEmpty(currentWord) && currentWord.StartsWith("@"))
        {
            _textToReplaceForAutocomplete = currentWord;
            var filter = currentWord.Substring(1);

            _autocompleteSuggestions = _allGlyphTypes
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

        await SyncEditorPills(forceCaretPos);
        StateHasChanged();
    }

    [JSInvokable("SelectSuggestionFromJS")]
    public async Task SelectSuggestionFromJS(string typeName)
    {
        var type = _allGlyphTypes.FirstOrDefault(t => t.Name == typeName);
        if (type != null) await SelectSuggestionByKeyboard(type);
    }

    #endregion

    #region UI Logic

    private async Task SyncEditorPills(int forceCaretPos = -1)
    {
        var fragments = _editorGlyph.GetTemplateFragments();
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

    private async Task HandlePillAction(NibContextAction action)
    {
        _isPillMenuVisible = false;
        _editorGlyph.HandleActionOnNib(action);
        await SyncEditorPills();
        StateHasChanged();
    }

    private async Task SelectSuggestionByKeyboard(Type selection)
    {
        await JsRuntime.InvokeVoidAsync("regexEditor.insertPill", _textToReplaceForAutocomplete, selection.Name, null, null);
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
        GlyphTypeRegistry.CreateAndRegisterNewTypeAndSaveToDisk(_editorGlyph);
        await OnClose.InvokeAsync(_editorGlyph);
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