using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace DocumentAnalysisInterface
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

        private bool _hideFullyMatchedDocuments;
        public bool HideFullyMatchedDocuments
        {
            get => _hideFullyMatchedDocuments;
            set
            {
                if (_hideFullyMatchedDocuments != value)
                {
                    _hideFullyMatchedDocuments = value;
                    OnChanged?.Invoke();
                    _ = DebouncedSaveAsync(); // Persist the change
                }
            }
        }
        
        private bool _showCoverageStats;
        public bool ShowCoverageStats
        {
            get => _showCoverageStats;
            set
            {
                if (_showCoverageStats != value)
                {
                    _showCoverageStats = value;
                    OnChanged?.Invoke();
                    _ = DebouncedSaveAsync(); // Persist the change
                }
            }
        }

        private bool _hideCollapsibleCaptureNodes;
        public bool HideCollapsibleCaptureNodes
        {
            get => _hideCollapsibleCaptureNodes;
            set
            {
                if (_hideCollapsibleCaptureNodes != value)
                {
                    _hideCollapsibleCaptureNodes = value;
                    OnChanged?.Invoke();
                    _ = DebouncedSaveAsync(); // Persist the change
                }
            }
        }

        /// <summary>
        /// Whether Corpus Captures spells declared class and property names out as prose ("Card
        /// Type") instead of showing them exactly as declared ("CardType"). Cosmetic only - the
        /// data-paths hover highlighting keys off are always built from the declared names. Off by
        /// default, so what's on screen matches what's in the glyph definitions.
        /// </summary>
        private bool _useFriendlyCaseNames;
        public bool UseFriendlyCaseNames
        {
            get => _useFriendlyCaseNames;
            set
            {
                if (_useFriendlyCaseNames != value)
                {
                    _useFriendlyCaseNames = value;
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

        private bool _orderByOccurrenceCount = true;
        public bool OrderByOccurrenceCount
        {
            get => _orderByOccurrenceCount;
            set
            {
                if (_orderByOccurrenceCount != value)
                {
                    _orderByOccurrenceCount = value;
                    OnChanged?.Invoke();
                    _ = DebouncedSaveAsync(); // Persist the change
                }
            }
        }

        private RegexDisplayMode _regexFormat = RegexDisplayMode.MatchedOnly;
        public RegexDisplayMode RegexFormat
        {
            get => _regexFormat;
            set
            {
                if (_regexFormat != value)
                {
                    _regexFormat = value;
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

        private FooterTrayContent _footerTrayContent = FooterTrayContent.Hidden;
        public FooterTrayContent FooterTrayContent
        {
            get => _footerTrayContent;
            set
            {
                if (_footerTrayContent != value)
                {
                    _footerTrayContent = value;
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

        private bool _showEchoes;
        public bool ShowEchoes
        {
            get => _showEchoes;
            set
            {
                if (_showEchoes != value)
                {
                    _showEchoes = value;
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
                    _hideFullyMatchedDocuments = dto.HideFullyMatchedDocuments;
                    _showCoverageStats = dto.ShowCoverageStats;
                    _hideCollapsibleCaptureNodes = dto.HideCollapsibleCaptureNodes;
                    _useFriendlyCaseNames = dto.UseFriendlyCaseNames;
                    _orderByWordCount = dto.OrderByWordCount;
                    _orderByOccurrenceCount = dto.OrderByOccurrenceCount;
                    _regexFormat = dto.RegexFormat;
                    _hideRegexesWithZeroCaptures = dto.HideRegexesWithZeroCaptures;
                    _hideBlankLines = dto.HideBlankLines;
                    _footerTrayContent = dto.FooterTrayContent;
                    _minSpanWords = dto.MinSpanWords;
                    _minSpanOccurences = dto.MinSpanOccurences;
                    _showEchoes = dto.ShowEchoes;
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
        public bool HideFullyMatchedDocuments { get; init; }
        public bool ShowCoverageStats { get; init; }
        public bool HideCollapsibleCaptureNodes { get; init; }
        public bool UseFriendlyCaseNames { get; init; }
        public bool OrderByWordCount { get; init; }
        public bool OrderByOccurrenceCount { get; init; } = true;
        public RegexDisplayMode RegexFormat { get; init; } = RegexDisplayMode.MatchedOnly;
        public bool HideRegexesWithZeroCaptures { get; init; }
        public bool HideBlankLines { get; init; }
        public FooterTrayContent FooterTrayContent { get; init; } = FooterTrayContent.Hidden;
        public int MinSpanWords { get; init; }
        public int MinSpanOccurences { get; init; }
        public bool ShowEchoes { get; init; }

        // Parameterless constructor for deserialization
        public RuntimeSettingsDto() { }

        public RuntimeSettingsDto(RuntimeSettings runtimeSettings)
        {
            HideFullyMatchedDocuments = runtimeSettings.HideFullyMatchedDocuments;
            ShowCoverageStats = runtimeSettings.ShowCoverageStats;
            HideCollapsibleCaptureNodes = runtimeSettings.HideCollapsibleCaptureNodes;
            UseFriendlyCaseNames = runtimeSettings.UseFriendlyCaseNames;
            OrderByWordCount = runtimeSettings.OrderByWordCount;
            OrderByOccurrenceCount = runtimeSettings.OrderByOccurrenceCount;
            RegexFormat = runtimeSettings.RegexFormat;
            HideRegexesWithZeroCaptures = runtimeSettings.HideRegexesWithZeroCaptures;
            HideBlankLines = runtimeSettings.HideBlankLines;
            FooterTrayContent = runtimeSettings.FooterTrayContent;
            MinSpanWords = runtimeSettings.MinSpanWords;
            MinSpanOccurences = runtimeSettings.MinSpanOccurences;
            ShowEchoes = runtimeSettings.ShowEchoes;
        }
    }
}