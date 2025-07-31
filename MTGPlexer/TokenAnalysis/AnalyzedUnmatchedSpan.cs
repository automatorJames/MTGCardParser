namespace MTGPlexer.TokenAnalysis;

/// <summary>
/// The final, enriched analysis of a single unique span of text (either full or maximal).
/// </summary>
public record AnalyzedUnmatchedSpan
{
    /// <summary>The text of the span.</summary>
    public string Text { get; init; }

    /// <summary>
    /// The number of times this EXACT sequence of words appears contiguously in the corpus.
    /// This value comes directly from the suffix automaton's frequency count.
    /// </summary>
    public int ExactPhraseFrequency { get; init; }

    /// <summary>
    /// The number of unique, longer original unmatched spans that this span is a sub-span of.
    /// This value comes from counting the list of found contexts.
    /// </summary>
    public int ContextualOccurrenceCount { get; init; }

    /// <summary>True if this span was one of the original, full unmatched spans from the tokenizer.</summary>
    public bool IsFullSpan { get; init; }

    /// <summary>
    /// A complete list of every place this span was found, linking back to the original full context.
    /// </summary>
    public List<SubSpanContext> Occurrences { get; init; }

    /// <summary>A hierarchical tree representing all token/word sequences that appeared immediately BEFORE this span.</summary>
    public List<AdjacencyNode> PrecedingAdjacencies { get; init; }

    /// <summary>A hierarchical tree representing all token/word sequences that appeared immediately AFTER this span.</summary>
    public List<AdjacencyNode> FollowingAdjacencies { get; init; }

    public int WordCount { get; init; }

    // Note the change in the parameter name from "frequency" to "exactPhraseFrequency" for clarity.
    public AnalyzedUnmatchedSpan(string text, int exactPhraseFrequency, bool isFullSpan, List<SubSpanContext> occurrences, List<AdjacencyNode> precedingAdjacencies, List<AdjacencyNode> followingAdjacencies)
    {
        Text = text;
        ExactPhraseFrequency = exactPhraseFrequency; // Store the automaton's count here.
        IsFullSpan = isFullSpan;
        Occurrences = occurrences;
        PrecedingAdjacencies = precedingAdjacencies;
        FollowingAdjacencies = followingAdjacencies;
        WordCount = text.Split(' ').Length;
        ContextualOccurrenceCount = occurrences.Count; // Store the contextual count here.
    }

    // Now your ToString() can be more precise.
    public override string ToString() => $"'{Text}' (Exact: {ExactPhraseFrequency}, In Contexts: {ContextualOccurrenceCount})";
}