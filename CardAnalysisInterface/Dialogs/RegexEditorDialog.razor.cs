using MTGPlexer.CommonDTOs;
using MTGPlexer.TokenAnalysisDTOs.SpanAnalysis;
using MTGPlexer.TokenUnitComponents;
using MTGPlexer.TokenUnits;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Text.RegularExpressions;

namespace CardAnalysisInterface.Dialogs;

public partial class RegexEditorDialog : ComponentBase, IAsyncDisposable
{
    [Parameter]
    public ProcessedLine Line { get; set; } = default!;

    [Parameter]
    public EventCallback<string> OnClose { get; set; }

    string _renderedRegex = "";
    string _className = $"New{nameof(TokenUnit)}";
    string _currentRawPattern = "";
    List<RegexSegment> _regexSegments = new();
    List<Match> _currentMatches = new();
    DynamicTokenType _dynamicTokenType;

    bool _isDropdownVisible = false;
    bool _isEditingClassName = false;
    bool _shouldFocusClassName = false;
    List<Type> _allTemplateTypes = new();
    List<Type> _autocompleteSuggestions = new();
    int _selectedSuggestionIndex = -1;
    bool _isEditorEmpty = true;
    bool _showPreviewBoxes = false;
    string _textToReplaceForAutocomplete = "";

    ElementReference _editorElement;
    DotNetObjectReference<RegexEditorDialog> _dotNetRef;

    public record RegexSegment(string Text, string Color);

    private enum MatchStatus { None, Partial, Full }
    private record TextSegment(string Text, string Color, string UnderlineClass);

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
            var colorMap = _allTemplateTypes.ToDictionary(
                t => t.Name,
                t => new {
                    Normal = DeterministicPalette.TypePaletteSet[t].Dark,
                    Highlight = DeterministicPalette.TypePaletteSet[t].Light
                }
            );
            await JsRuntime.InvokeVoidAsync("initializeEditor", _dotNetRef, _editorElement, colorMap);
        }

        if (_isEditingClassName && _shouldFocusClassName)
        {
            _shouldFocusClassName = false;
            await JsRuntime.InvokeVoidAsync("eval", @"
                (function() {
                    const el = document.querySelector('.class-name-input');
                    if (el) {
                        el.focus();
                        const val = el.value;
                        el.value = '';
                        el.value = val;
                    }
                })()");
        }
    }

    private void StartEditingClassName()
    {
        _isEditingClassName = true;
        _shouldFocusClassName = true;
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

    private List<TextSegment> GetProcessedSegments()
    {
        var segments = new List<TextSegment>();
        string text = Line.SourceText.FormattedText;
        if (string.IsNullOrEmpty(text)) return segments;

        var charStatus = new MatchStatus[text.Length];
        for (int i = 0; i < text.Length; i++) charStatus[i] = MatchStatus.None;

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
                {
                    for (int k = Math.Max(mStart, word.Start); k <= Math.Min(mEnd, word.End); k++)
                        charStatus[k] = MatchStatus.Full;
                }
                else
                {
                    for (int k = Math.Max(mStart, word.Start); k <= Math.Min(mEnd, word.End); k++)
                        if (charStatus[k] == MatchStatus.None) charStatus[k] = MatchStatus.Partial;
                }
            }

            for (int k = mStart; k <= mEnd; k++)
            {
                if (charStatus[k] == MatchStatus.None)
                {
                    bool leftFull = k > 0 && charStatus[k - 1] == MatchStatus.Full;
                    bool rightFull = k < text.Length - 1 && charStatus[k + 1] == MatchStatus.Full;

                    if (leftFull && rightFull && char.IsWhiteSpace(text[k]))
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
        _currentRawPattern = rawText;

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

    [JSInvokable]
    public async Task SelectSuggestionFromJS(string typeName)
    {
        var type = _allTemplateTypes.FirstOrDefault(t => t.Name == typeName);
        if (type != null)
        {
            await SelectSuggestionByKeyboard(type);
        }
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
                    await SelectSuggestionByKeyboard(_autocompleteSuggestions[_selectedSuggestionIndex]);
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
        _currentMatches.Clear();
        var logicalPattern = patternToRender.Trim();
        _showPreviewBoxes = logicalPattern.Contains("@");

        if (string.IsNullOrWhiteSpace(logicalPattern))
        {
            _renderedRegex = "";
            _regexSegments.Clear();
            return;
        }

        try
        {
            if (_showPreviewBoxes)
            {
                var cleanPattern = logicalPattern.Replace('\u00A0', ' ');
                _dynamicTokenType = new DynamicTokenType(cleanPattern, className: _className);
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
                    _renderedRegex = ex.Message;
                    _regexSegments = new List<RegexSegment> { new(_renderedRegex, "var(--error-red)") };
                }
            }
        }
        catch
        {
            _renderedRegex = "Error rendering template";
            _regexSegments = new List<RegexSegment> { new(_renderedRegex, "var(--error-red)") };
        }
    }

    private void ParseSegments()
    {
        _regexSegments.Clear();
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
                        _regexSegments.Add(new(_renderedRegex.Substring(lastPos, i - lastPos), "var(--syntax-default)"));
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

                    _regexSegments.Add(new(groupText, color));
                    lastPos = i + 1;
                }
            }
        }

        if (lastPos < _renderedRegex.Length)
            _regexSegments.Add(new(_renderedRegex.Substring(lastPos), "var(--syntax-default)"));
    }

    private async Task SaveClassToFile()
    {
        if (_dynamicTokenType != null)
            TokenTypeRegistry.CreateAndRegisterNewTypeAndSaveToDisk(_dynamicTokenType);

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