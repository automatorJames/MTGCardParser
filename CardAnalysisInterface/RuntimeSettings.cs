namespace CardAnalysisInterface;

public class RuntimeSettings
{
    public event Action OnChanged;

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
            }
        }
    }
}