namespace MTGPlexer.TokenAnalysis;

/// <summary>
/// The primary output record. Represents a unique unmatched span text, its total count
/// across the corpus, and a list of every specific occurrence with its context.
/// This fulfills the "List<Something>" requirement.
/// </summary>
public record AnalyzedUnmatchedSpan
{
    /// <summary>
    /// The text of the unmatched span (e.g., "target creature" or "destroy target creature").
    /// </summary>
    public string Text { get; init; }

    /// <summary>
    /// The total number of times this exact text sequence appears in the corpus.
    /// </summary>
    public int OccurrenceCount { get; init; }

    /// <summary>
    /// The number of words in the span. Calculated once.
    /// </summary>
    public int WordCount { get; init; }

    /// <summary>
    /// True if this span was one of the original, full unmatched spans from a card.
    /// </summary>
    public bool IsOriginalFullSpan { get; init; }

    /// <summary>
    /// A complete list of every place this span text was found, including the card,
    /// line, and the immediate preceding/following tokens for that specific instance.
    /// </summary>
    public List<UnmatchedSpanOccurrence> Occurrences { get; init; }

    public List<SpanAdjacentWord> PrecedingWords { get; init; }
    public List<SpanAdjacentWord> FollowingWords { get; init; }

    public AnalyzedUnmatchedSpan(string text, int occurrenceCount, bool isOriginalFullSpan, List<UnmatchedSpanOccurrence> occurrences)
    {
        Text = text;
        OccurrenceCount = occurrenceCount;
        WordCount = text.Split(' ').Length;
        IsOriginalFullSpan = isOriginalFullSpan;
        Occurrences = occurrences;

        Dictionary<string, int> precedingWordCounts = [];
        Dictionary<string, int> followingWordCounts = [];

        foreach (var occurrence in occurrences)
        {
            if (occurrence.PrecedingWord != null)
                if (!precedingWordCounts.TryAdd(occurrence.PrecedingWord, 1))
                    precedingWordCounts[occurrence.PrecedingWord]++;

            if (occurrence.FollowingWord != null)
                if (!followingWordCounts.TryAdd(occurrence.FollowingWord, 1))
                    followingWordCounts[occurrence.FollowingWord]++;
        }

        PrecedingWords = precedingWordCounts
            .OrderByDescending(x => x.Value)
            .Select(x => new SpanAdjacentWord(x.Key, x.Value))
            .ToList();

        FollowingWords = followingWordCounts
            .OrderByDescending(x => x.Value)
            .Select(x => new SpanAdjacentWord(x.Key, x.Value))
            .ToList();
    }

    public override string ToString() => $"'{Text}' (x{OccurrenceCount})";
}

