using System.Text.RegularExpressions;
using MTGPlexer.CommonDTOs;
using MTGPlexer.TokenAnalysisDTOs.SpanAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace CardAnalysisInterface.Dialogs;

public partial class RegexEditorDialog : ComponentBase, IAsyncDisposable
{
    [Parameter]
    public ProcessedLine Line { get; set; } = default!;

    [Parameter]
    public EventCallback<string> OnClose { get; set; }

    private string _renderedRegex = "";
    private List<RegexSegment> _regexSegments = new();
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

    public record RegexSegment(string Text, string Color);

    protected override void OnInitialized()
    {
        _dotNetRef = DotNetObjectReference.Create(this);

        _allTemplateTypes.Clear();
        _allTemplateTypes.AddRange(TokenTypeRegistry.AppliedOrderTypes);
        _allTemplateTypes.AddRange(TokenTypeRegistry.ReferencedEnumTypes);
        _allTemplateTypes = _allTemplateTypes.OrderBy(t => t.Name).ToList();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && _dotNetRef != null)
        {
            // Pass the palette colors to JS so the tokens can be colored correctly
            var colorMap = _allTemplateTypes.ToDictionary(
                t => t.Name,
                t => new {
                    Normal = DeterministicPalette.TypePaletteSet[t].Dark,
                    Highlight = DeterministicPalette.TypePaletteSet[t].Light
                }
            );
            await JsRuntime.InvokeVoidAsync("initializeEditor", _dotNetRef, _editorElement, colorMap);
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
                _renderedRegex = _dynamicTokenType.RenderedRegex;
            }
            else
            {
                _renderedRegex = logicalPattern;
                _dynamicTokenType = null;
            }

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
                    _currentMatches.Clear();
                    _renderedRegex = ex.Message;
                    _regexSegments = new List<RegexSegment> { new(_renderedRegex, "#F87171") };
                }
            }
        }
        catch
        {
            _currentMatches.Clear();
            _renderedRegex = "Error rendering template";
            _regexSegments = new List<RegexSegment> { new(_renderedRegex, "#F87171") };
        }
    }

    private void ParseSegments()
    {
        _regexSegments.Clear();
        if (string.IsNullOrEmpty(_renderedRegex)) return;

        // Algorithm to find top-level capture groups
        int depth = 0;
        int lastPos = 0;

        for (int i = 0; i < _renderedRegex.Length; i++)
        {
            if (_renderedRegex[i] == '\\') { i++; continue; } // skip escaped

            if (_renderedRegex[i] == '(')
            {
                if (depth == 0)
                {
                    // Add text before the group
                    if (i > lastPos)
                        _regexSegments.Add(new(_renderedRegex.Substring(lastPos, i - lastPos), "#d4d4d4"));
                    lastPos = i;
                }
                depth++;
            }
            else if (_renderedRegex[i] == ')')
            {
                depth--;
                if (depth == 0)
                {
                    // Found end of top-level group
                    var groupText = _renderedRegex.Substring(lastPos, i - lastPos + 1);
                    var match = Regex.Match(groupText, @"^\(\?<(?<name>[a-zA-Z0-9_]+)>");

                    string color = "#d4d4d4";
                    if (match.Success)
                    {
                        var name = match.Groups["name"].Value;
                        if (TokenTypeRegistry.NameToType.TryGetValue(name, out var type))
                            color = DeterministicPalette.TypePaletteSet[type].Normal;
                    }

                    _regexSegments.Add(new(groupText, color));
                    lastPos = i + 1;
                }
            }
        }

        if (lastPos < _renderedRegex.Length)
            _regexSegments.Add(new(_renderedRegex.Substring(lastPos), "#d4d4d4"));
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
            try { await JsRuntime.InvokeVoidAsync("disposeEditor"); } catch { }
            _dotNetRef.Dispose();
        }
    }
}