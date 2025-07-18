namespace CardAnalysisInterface;

public record RegexPatternDisplaySegment
(
    string OriginalText,
    bool IsPill = false
)
{
    public Guid Id { get; } = Guid.NewGuid();
    public string DisplayText { get; } = IsPill ? OriginalText.Substring(1) : OriginalText;
}