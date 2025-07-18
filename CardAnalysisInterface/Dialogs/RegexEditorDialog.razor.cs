using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CardAnalysisInterface.Dialogs
{
    public partial class RegexEditorDialog : ComponentBase, IAsyncDisposable
    {
        [Parameter]
        public CardLine Line { get; set; }

        [Parameter]
        public EventCallback<string> OnClose { get; set; }

        private string _renderedRegex = "";
        private List<Match> _currentMatches = new List<Match>();
        private TokenTemplatePreview _tokenTemplatePreview;

        private bool _isDropdownVisible = false;
        private List<Type> _allTemplateTypes = new();
        private List<Type> _autocompleteSuggestions = new();
        private int _selectedSuggestionIndex = -1;
        private bool _isEditorEmpty = true;
        private bool _showPreviewBoxes = false;

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
        public void UpdateFromJavaScript(string rawText, string currentWord)
        {
            _isEditorEmpty = string.IsNullOrEmpty(rawText);

            if (currentWord.StartsWith("@"))
            {
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
            }

            UpdateRenderedRegexAndMatches(rawText);
            StateHasChanged();
        }

        private Task OnKeyDown(KeyboardEventArgs e)
        {
            if (!_isDropdownVisible)
            {
                return Task.CompletedTask;
            }

            switch (e.Key)
            {
                case "ArrowDown":
                    _selectedSuggestionIndex = (_selectedSuggestionIndex + 1) % _autocompleteSuggestions.Count;
                    break;
                case "ArrowUp":
                    _selectedSuggestionIndex = (_selectedSuggestionIndex - 1 + _autocompleteSuggestions.Count) % _autocompleteSuggestions.Count;
                    break;
                case "Enter":
                case "Tab":
                    if (_selectedSuggestionIndex != -1 && _autocompleteSuggestions.Count > _selectedSuggestionIndex)
                    {
                        // This path is now only for keyboard.
                        SelectSuggestion(_autocompleteSuggestions[_selectedSuggestionIndex]);
                    }
                    break;
                case "Escape":
                    _isDropdownVisible = false;
                    break;
            }

            StateHasChanged();
            return Task.CompletedTask;
        }

        private async void SelectSuggestion(Type selection)
        {
            // This method is now only for keyboard events (Enter/Tab).
            // It calls the JS insertion function, which will get the current caret info itself.
            var pillDisplayName = selection.Name;
            var pillRawText = $"@{pillDisplayName}";
            var pillColor = TokenTypeRegistry.Palettes[selection].HexDark;

            await JsRuntime.InvokeVoidAsync("insertPillIntoEditor", pillDisplayName, pillRawText, pillColor);
        }

        private void UpdateRenderedRegexAndMatches(string patternToRender)
        {
            var logicalPattern = patternToRender.Replace("\u200B", "").Trim();

            _showPreviewBoxes = logicalPattern.Contains("@");

            if (!_showPreviewBoxes)
            {
                _renderedRegex = logicalPattern;
                _tokenTemplatePreview = null;
            }

            try
            {
                if (_showPreviewBoxes)
                {
                    _tokenTemplatePreview = new TokenTemplatePreview(logicalPattern);
                    _renderedRegex = _tokenTemplatePreview.RenderedRegex;
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
            var finalPattern = await JsRuntime.InvokeAsync<string>("getEditorRawText");
            await OnClose.InvokeAsync(finalPattern.Trim());
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
}