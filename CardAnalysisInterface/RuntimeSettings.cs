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
}