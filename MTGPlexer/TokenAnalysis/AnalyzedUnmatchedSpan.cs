namespace MTGPlexer.TokenAnalysis;

/// <summary>
/// The final, enriched analysis of a single unique span of text (either full or maximal).
/// </summary>
public record AnalyzedUnmatchedSpan
{
    /// <summary>The text of the span.</summary>
    public string Text { get; init; }

    /// <summary>How many times this exact span appeared in the corpus.</summary>
    public int Frequency { get; init; }

    /// <summary>True if this span was one of the original, full unmatched spans from the tokenizer.</summary>
    public bool IsFullSpan { get; init; }

    /// <summary>
    /// A complete list of every place this span was found, linking back to the original full context.
    /// </summary>
    public List<SubSpanContext> Occurrences { get; init; }

    /// <summary>A frequency list of all items that appeared immediately before this span.</summary>
    public List<SpanAdjacency> PrecedingAdjacencies { get; init; }

    /// <summary>A frequency list of all items that appeared immediately after this span.</summary>
    public List<SpanAdjacency> FollowingAdjacencies { get; init; }

    public int WordCount { get; init; }
    public int OccurrenceCount { get; init; }

    public AnalyzedUnmatchedSpan(string text, int frequency, bool isFullSpan, List<SubSpanContext> occurrences, List<SpanAdjacency> precedingAdjacencies, List<SpanAdjacency> followingAdjacencies)
    {
        Text = text;
        Frequency = frequency;
        IsFullSpan = isFullSpan;
        Occurrences = occurrences;
        PrecedingAdjacencies = precedingAdjacencies;
        FollowingAdjacencies = followingAdjacencies;
        WordCount = text.Split(' ').Length;
        OccurrenceCount = occurrences.Count;
    }

    public override string ToString() => $"'{Text}' (x{OccurrenceCount})";
}

