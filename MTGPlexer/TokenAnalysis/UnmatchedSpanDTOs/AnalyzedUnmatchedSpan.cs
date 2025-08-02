namespace MTGPlexer.TokenAnalysis.UnmatchedSpanDTOs;

/// <summary>
/// Enriched analysis of a single unique span of unmatched text.
/// </summary>
public record AnalyzedUnmatchedSpan
{
    /// <summary>The text of the span.</summary>
    public string Text { get; init; }

    /// <summary>
    /// The number of times this EXACT sequence of unmatched Text appears contiguously in the corpus
    /// as a maximal span, meaning that increasing the span's lengeth on either side would reduce its
    /// occurrence count.
    /// </summary>
    public int MaximalSpanOccurrenceCount { get; init; }

    /// <summary>
    /// This total corpus-wide ocurrence count of this Text. This includes occurrences where the whole unmatched Text
    /// comprises exactly this Text in its entirety, and occurrences where this Text is a sub-span within some larger
    /// unmatched Text.
    /// </summary>
    public int TotalOccurrenceCount { get; init; }

    /// <summary>True if this span was one of the original, full unmatched spans from the tokenizer.</summary>
    public bool IsFullSpan { get; init; }

    /// <summary>
    /// A complete list of every place this span was found, linking back to the original full context.
    /// </summary>
    public List<UnmatchedSubSpanContext> Occurrences { get; init; }

    /// <summary>A hierarchical tree representing all token/word sequences that appeared immediately BEFORE this span.</summary>
    public List<AdjacencyNode> PrecedingAdjacencies { get; init; }

    /// <summary>A hierarchical tree representing all token/word sequences that appeared immediately AFTER this span.</summary>
    public List<AdjacencyNode> FollowingAdjacencies { get; init; }

    public int WordCount { get; init; }

    public Dictionary<string, int> OccurrencesPerCard => Occurrences
        .GroupBy(x => x.OriginalOccurrence.CardName)
        .OrderByDescending(x => x.Count())
        .ThenBy(x => x.Key)
        .ToDictionary(x => x.Key, x => x.Count());

    // Note the change in the parameter name from "frequency" to "exactPhraseFrequency" for clarity.
    public AnalyzedUnmatchedSpan(
        string text, 
        int maximalSpanOccurrenceCount, 
        bool isFullSpan, 
        List<UnmatchedSubSpanContext> occurrences, 
        List<AdjacencyNode> precedingAdjacencies, 
        List<AdjacencyNode> followingAdjacencies)
    {
        Text = text;
        MaximalSpanOccurrenceCount = maximalSpanOccurrenceCount; // Store the automaton's count here.
        IsFullSpan = isFullSpan;
        Occurrences = occurrences;
        PrecedingAdjacencies = precedingAdjacencies;
        FollowingAdjacencies = followingAdjacencies;
        WordCount = text.Split(' ').Length;
        TotalOccurrenceCount = occurrences.Count; // Store the contextual count here.
    }

    // Now your ToString() can be more precise.
    public override string ToString() => $"'{Text}' (Total: {TotalOccurrenceCount} | Maximal: {MaximalSpanOccurrenceCount})";
}