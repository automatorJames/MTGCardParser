namespace MTGPlexer.TokenAnalysis.UnmatchedSpanDTOs;

/// <summary>
/// A lightweight pointer that precisely identifies a sub-span within a specific UnmatchedSpanOccurrence.
/// This is the key to linking maximal spans back to their full, original context.
/// </summary>
/// <param name="OriginalOccurrence">The ground-truth occurrence where the span was found.</param>
/// <param name="WordStartIndex">The starting word index of the sub-span within the OriginalOccurrence's SpanWords.</param>
/// <param name="WordCount">The number of words in the sub-span.</param>
public record UnmatchedSubSpanContext(UnmatchedSpanOccurrence OriginalOccurrence, int WordStartIndex, int WordCount)
{
    public override string ToString() =>
        "("
        + string.Join(' ', OriginalOccurrence.Words.Skip(WordStartIndex).Take(WordCount))
        + ") "
        + OriginalOccurrence.Text;
}

