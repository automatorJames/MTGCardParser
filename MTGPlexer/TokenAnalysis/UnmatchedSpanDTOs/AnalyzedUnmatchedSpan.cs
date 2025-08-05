namespace MTGPlexer.TokenAnalysis;

/// <summary>
/// Enriched analysis of a single unique span of unmatched text.
/// This object is now prepared at construction to be passed directly to JavaScript for visualization.
/// </summary>
public record AnalyzedUnmatchedSpan
{
    /// <summary>The text of the span. This serves as the anchor text in the visualization.</summary>
    [JsonPropertyName("text")]
    public string Text { get; init; }
    
    /// <summary>A hierarchical tree representing all token/word sequences that appeared immediately BEFORE this span.</summary>
    [JsonPropertyName("precedingAdjacencies")]
    public List<AdjacencyNode> PrecedingAdjacencies { get; init; }
    
    /// <summary>A hierarchical tree representing all token/word sequences that appeared immediately AFTER this span.</summary>
    [JsonPropertyName("followingAdjacencies")]
    public List<AdjacencyNode> FollowingAdjacencies { get; init; }
    
    /// <summary>A dictionary mapping Card Names to their designated hex color codes for the UI.</summary>
    [JsonPropertyName("cardColors")]
    public Dictionary<string, string> CardColors { get; init; }
    
    // --- Properties Not Directly Used by JS Visualization (but still useful) ---
    
    [JsonIgnore]
    public int MaximalSpanOccurrenceCount { get; init; }
    
    [JsonIgnore]
    public int TotalOccurrenceCount { get; init; }
    
    /// <summary>
    /// A complete list of every place this span was found.
    /// This is kept for data inspection but not sent to the client by default.
    /// </summary>
    [JsonIgnore]
    public List<UnmatchedSubSpanContext> Occurrences { get; init; }
    
    [JsonIgnore]
    public int WordCount { get; init; }
    
    [JsonIgnore]
    public Dictionary<string, int> OccurrencesPerCard => Occurrences
        .GroupBy(x => x.OriginalOccurrence.Key.CardName)
        .OrderByDescending(x => x.Count())
        .ThenBy(x => x.Key)
        .ToDictionary(x => x.Key, x => x.Count());
    
    public AnalyzedUnmatchedSpan(
        string text,
        int maximalSpanOccurrenceCount,
        List<UnmatchedSubSpanContext> occurrences,
        List<AdjacencyNode> precedingAdjacencies,
        List<AdjacencyNode> followingAdjacencies)
    {
        // --- Standard Initializations ---
        Text = text;
        MaximalSpanOccurrenceCount = maximalSpanOccurrenceCount;
        Occurrences = occurrences;
        PrecedingAdjacencies = precedingAdjacencies;
        FollowingAdjacencies = followingAdjacencies;
        WordCount = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
        TotalOccurrenceCount = occurrences.Count;
    
        // --- Color Hydration ---
        // Populate the CardColors dictionary using the centrally-defined palettes.
        CardColors = OccurrencesPerCard.Keys
            .ToDictionary(
                cardName => cardName,
                cardName => TokenTypeRegistry.CorpusItemPalettes.TryGetValue(cardName, out var palette)
                            ? palette.Hex // Assumes DeterministicPalette has a .CssColor string property
                            : "#dddddd"       // Fallback color for safety
            );

        // --- HYDRATION STEP ---
        // Instead of transforming into new DTOs, we now "hydrate" the existing
        // AdjacencyNode trees with the unique IDs required by the JS for DOM manipulation.
        // This is a one-time operation that makes this entire object "JS-ready".
        int nodeIdCounter = 0;
        void HydrateTreeWithIds(IEnumerable<AdjacencyNode> nodes)
        {
            foreach (var node in nodes)
            {
                // Set the mutable 'Id' property on the node.
                node.Id = $"n{nodeIdCounter++}";
                HydrateTreeWithIds(node.Children);
            }
        }
    
        HydrateTreeWithIds(PrecedingAdjacencies);
        HydrateTreeWithIds(FollowingAdjacencies);
    }

    public override string ToString() => $"'{Text}' (Total: {TotalOccurrenceCount} | Maximal: {MaximalSpanOccurrenceCount})";
}