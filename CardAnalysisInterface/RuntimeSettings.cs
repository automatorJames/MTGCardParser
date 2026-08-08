using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CardAnalysisInterface
{
    public class RuntimeSettings : IAsyncDisposable
    {
        private const string Key = "runtime-settings";
        private readonly ProtectedLocalStorage _pls;
        private bool _loaded;
        private CancellationTokenSource _saveCts;

        /// <summary>
        /// Public flag to indicate if the initial settings have been loaded from storage.
        /// </summary>
        public bool IsLoaded => _loaded;

        /// <summary>
        /// Event that fires when settings values change.
        /// </summary>
        public event Action OnChanged;

        public RuntimeSettings(ProtectedLocalStorage pls) => _pls = pls;

        private bool _hideFullyMatchedCards;
        public bool HideFullyMatchedCards
        {
            get => _hideFullyMatchedCards;
            set
            {
                if (_hideFullyMatchedCards != value)
                {
                    _hideFullyMatchedCards = value;
                    OnChanged?.Invoke();
                    _ = DebouncedSaveAsync(); // Persist the change
                }
            }
        }
        
        private bool _orderByWordCount;
        public bool OrderByWordCount
        {
            get => _orderByWordCount;
            set
            {
                if (_orderByWordCount != value)
                {
                    _orderByWordCount = value;
                    OnChanged?.Invoke();
                    _ = DebouncedSaveAsync(); // Persist the change
                }
            }
        }

        private bool _showOriginalText;
        public bool ShowOriginalText
        {
            get => _showOriginalText;
            set
            {
                if (_showOriginalText != value)
                {
                    _showOriginalText = value;
                    OnChanged?.Invoke();
                    _ = DebouncedSaveAsync(); // Persist the change
                }
            }
        }

        private bool _showMinified;
        public bool ShowMinified
        {
            get => _showMinified;
            set
            {
                if (_showMinified != value)
                {
                    _showMinified = value;
                    OnChanged?.Invoke();
                    _ = DebouncedSaveAsync(); // Persist the change
                }
            }
        }

        private bool _hideBlankLines;
        public bool HideBlankLines
        {
            get => _hideBlankLines;
            set
            {
                if (_hideBlankLines != value)
                {
                    _hideBlankLines = value;
                    OnChanged?.Invoke();
                    _ = DebouncedSaveAsync(); // Persist the change
                }
            }
        }

        private bool _hideTypeTree;
        public bool HideTypeTree
        {
            get => _hideTypeTree;
            set
            {
                if (_hideTypeTree != value)
                {
                    _hideTypeTree = value;
                    OnChanged?.Invoke();
                    _ = DebouncedSaveAsync(); // Persist the change
                }
            }
        }

        private bool _hideRegexesWithZeroCaptures;
        public bool HideRegexesWithZeroCaptures
        {
            get => _hideRegexesWithZeroCaptures;
            set
            {
                if (_hideRegexesWithZeroCaptures != value)
                {
                    _hideRegexesWithZeroCaptures = value;
                    OnChanged?.Invoke();
                    _ = DebouncedSaveAsync(); // Persist the change
                }
            }
        }

        private int _minSpanWords = 3;
        public int MinSpanWords
        {
            get => _minSpanWords;
            set
            {
                if (_minSpanWords != value)
                {
                    _minSpanWords = value;
                    OnChanged?.Invoke();
                    _ = DebouncedSaveAsync(); // Persist the change
                }
            }
        }

        private int _minSpanOccurences = 3;
        public int MinSpanOccurences
        {
            get => _minSpanOccurences;
            set
            {
                if (_minSpanOccurences != value)
                {
                    _minSpanOccurences = value;
                    OnChanged?.Invoke();
                    _ = DebouncedSaveAsync(); // Persist the change
                }
            }
        }

        private string _searchTerm = string.Empty;
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                var newTerm = value ?? string.Empty;
                if (_searchTerm != newTerm)
                {
                    _searchTerm = newTerm;
                    OnChanged?.Invoke();
                }
            }
        }

        /// <summary>
        /// Loads settings from ProtectedLocalStorage. Should only be called once.
        /// </summary>
        public async Task EnsureLoadedAsync()
        {
            if (_loaded) return;

            try
            {
                var result = await _pls.GetAsync<RuntimeSettingsDto>(Key);
                if (result.Success && result.Value is { } dto)
                {
                    _hideFullyMatchedCards = dto.HideFullyMatchedCards;
                    _orderByWordCount = dto.OrderByWordCount;
                    _showOriginalText = dto.ShowOriginalText;
                    _showMinified = dto.ShowMinified;
                    _hideRegexesWithZeroCaptures = dto.HideRegexesWithZeroCaptures;
                    _hideBlankLines = dto.HideBlankLines;
                    _hideTypeTree = dto.HideTypeTree;
                    _minSpanWords = dto.MinSpanWords;
                    _minSpanOccurences = dto.MinSpanOccurences;
                }
            }
            catch
            {

            }

            // Mark as loaded AFTER attempting to load, so we don't try again.
            _loaded = true;

            // Notify all subscribers that the settings are now final (either default or loaded).
            OnChanged?.Invoke();
        }

        private async Task DebouncedSaveAsync()
        {
            _saveCts?.Cancel();
            _saveCts = new CancellationTokenSource();
            var token = _saveCts.Token;

            try
            {
                await Task.Delay(200, token); // debounce bursty toggles
                await _pls.SetAsync(Key, new RuntimeSettingsDto(this));
            }
            catch (TaskCanceledException) { /* expected when re-debouncing */ }
        }

        public async ValueTask DisposeAsync()
        {
            // Flush any pending save on circuit end
            _saveCts?.Cancel();
            if (_saveCts != null)
            {
                await DebouncedSaveAsync();
            }
        }
    }

    /// <summary>
    /// Data Transfer Object for persisting settings.
    /// </summary>
    public record RuntimeSettingsDto
    {
        public bool HideFullyMatchedCards { get; init; }
        public bool OrderByWordCount { get; init; }
        public bool ShowOriginalText { get; init; }
        public bool ShowMinified { get; init; }
        public bool HideRegexesWithZeroCaptures { get; init; }
        public bool HideBlankLines { get; init; }
        public bool HideTypeTree { get; init; }
        public int MinSpanWords { get; init; }
        public int MinSpanOccurences { get; init; }

        // Parameterless constructor for deserialization
        public RuntimeSettingsDto() { }

        public RuntimeSettingsDto(RuntimeSettings runtimeSettings)
        {
            HideFullyMatchedCards = runtimeSettings.HideFullyMatchedCards;
            OrderByWordCount = runtimeSettings.OrderByWordCount;
            ShowOriginalText = runtimeSettings.ShowOriginalText;
            ShowMinified = runtimeSettings.ShowMinified;
            HideRegexesWithZeroCaptures = runtimeSettings.HideRegexesWithZeroCaptures;
            HideBlankLines = runtimeSettings.HideBlankLines;
            HideTypeTree = runtimeSettings.HideTypeTree;
            MinSpanWords = runtimeSettings.MinSpanWords;
            MinSpanOccurences = runtimeSettings.MinSpanOccurences;
        }
    }
}