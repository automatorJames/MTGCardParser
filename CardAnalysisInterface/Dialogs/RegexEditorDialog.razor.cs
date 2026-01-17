using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MTGPlexer.CommonDTOs;
using MTGPlexer.TokenAnalysisDTOs.SpanAnalysis;
using System.Text.RegularExpressions;

namespace CardAnalysisInterface.Dialogs;

public partial class RegexEditorDialog : ComponentBase, IAsyncDisposable
{
    [Parameter]
    public ProcessedLine Line { get; set; } = default!;

    [Parameter]
    public EventCallback<string> OnClose { get; set; }

    private string _renderedRegex = "";
    private List<Match> _currentMatches = new();
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

        // Populate autocomplete source
        _allTemplateTypes.Clear();
        _allTemplateTypes.AddRange(TokenTypeRegistry.AppliedOrderTypes);
        _allTemplateTypes.AddRange(TokenTypeRegistry.ReferencedEnumTypes);
        _allTemplateTypes = _allTemplateTypes.OrderBy(t => t.Name).ToList();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && _dotNetRef != null)
        {
            await JsRuntime.InvokeVoidAsync("initializeEditor", _dotNetRef, _editorElement);
        }
    }

    [JSInvokable]
    public void HideDropdown()
    {
        if (_isDropdownVisible)
        {
            _isDropdownVisible = false;
            StateHasChanged();
        }
    }

    [JSInvokable]
    public async Task HandleGlobalEscape()
    {
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

    [JSInvokable]
    public void UpdateFromJavaScript(string rawText, string currentWord)
    {
        _isEditorEmpty = string.IsNullOrWhiteSpace(rawText);

        if (!string.IsNullOrEmpty(currentWord) && currentWord.StartsWith("@"))
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
        if (!_isDropdownVisible) return;

        switch (e.Key)
        {
            case "ArrowDown":
                _selectedSuggestionIndex = (_selectedSuggestionIndex + 1) % _autocompleteSuggestions.Count;
                await JsRuntime.InvokeVoidAsync("scrollToAutocompleteItem", $"autocomplete-item-{_selectedSuggestionIndex}");
                break;
            case "ArrowUp":
                _selectedSuggestionIndex = (_selectedSuggestionIndex - 1 + _autocompleteSuggestions.Count) % _autocompleteSuggestions.Count;
                await JsRuntime.InvokeVoidAsync("scrollToAutocompleteItem", $"autocomplete-item-{_selectedSuggestionIndex}");
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

    // Renamed back to SelectSuggestionByKeyboard to match the Razor template
    private async Task SelectSuggestionByKeyboard(Type selection)
    {
        string fullTokenText = $"@{selection.Name}";
        await JsRuntime.InvokeVoidAsync("commitToken", _textToReplaceForAutocomplete, fullTokenText);
        _isDropdownVisible = false;
        StateHasChanged();
    }

    private void UpdateRenderedRegexAndMatches(string patternToRender)
    {
        var logicalPattern = patternToRender.Trim();
        _showPreviewBoxes = logicalPattern.Contains("@");

        try
        {
            if (_showPreviewBoxes)
            {
                var cleanPattern = logicalPattern.Replace('\u00A0', ' ');
                _dynamicTokenType = new DynamicTokenType(cleanPattern);
                _renderedRegex = _dynamicTokenType.GetRenderedRegexFromTemplate();
            }
            else
            {
                _renderedRegex = logicalPattern;
                _dynamicTokenType = null;
            }

            if (!string.IsNullOrWhiteSpace(_renderedRegex) && _renderedRegex != "Error: Invalid template")
            {
                _currentMatches = Regex.Matches(Line.SourceText.FormattedText, _renderedRegex)
                                       .Cast<Match>()
                                       .ToList();
            }
            else
            {
                _currentMatches.Clear();
            }
        }
        catch
        {
            _renderedRegex = "Error: Invalid template";
            _currentMatches.Clear();
        }
    }

    private async Task HandleSubmit()
    {
        if (_dynamicTokenType != null)
        {
            TokenTypeRegistry.AddNewTypeAndSaveToDisk(_dynamicTokenType);
        }
        await OnClose.InvokeAsync(_renderedRegex);
    }

    private Task HandleCancel() => OnClose.InvokeAsync(null);

    public async ValueTask DisposeAsync()
    {
        if (_dotNetRef != null)
        {
            try
            {
                await JsRuntime.InvokeVoidAsync("disposeEditor");
            }
            catch { /* Ignore JS errors during disposal */ }

            _dotNetRef.Dispose();
        }
    }
}