namespace MTGPlexer.TokenAnalysis.UnmatchedSpanDTOs;

/// <summary>
/// A lightweight pointer that precisely identifies a sub-span within a specific UnmatchedSpanOccurrence.
/// This is the key to linking maximal spans back to their full, original context.
/// </summary>
public record UnmatchedSubSpanContext
{
    public UnmatchedSpanOccurrence OriginalOccurrence { get; }
    public int WordStartIndex { get; }
    public int WordCount { get; }
    public string Text { get; }

    public UnmatchedSubSpanContext(UnmatchedSpanOccurrence originalOccurrence, int wordStartIndex, int wordCount)
    {
        OriginalOccurrence = originalOccurrence;
        WordStartIndex = wordStartIndex;
        WordCount = wordCount;
        Text = string.Join(' ', OriginalOccurrence.Words.Skip(WordStartIndex).Take(WordCount));
    }

    public override string ToString() => $"({Text}) {OriginalOccurrence.Text}";
}