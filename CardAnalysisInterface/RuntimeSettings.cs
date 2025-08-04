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

    private int _minUnmatchedSpanWords = 3;
    public int MinUnmatchedSpanWords
    {
        get => _minUnmatchedSpanWords;
        set
        {
            if (_minUnmatchedSpanWords != value)
            {
                _minUnmatchedSpanWords = value;
                OnChanged?.Invoke();
            }
        }
    }

    private int _minUnmatchedSpanOccurences = 3;
    public int MinUnmatchedSpanOccurences
    {
        get => _minUnmatchedSpanOccurences;
        set
        {
            if (_minUnmatchedSpanOccurences != value)
            {
                _minUnmatchedSpanOccurences = value;
                OnChanged?.Invoke();
            }
        }
    }
}