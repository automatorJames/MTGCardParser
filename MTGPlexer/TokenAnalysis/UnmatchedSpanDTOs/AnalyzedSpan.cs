namespace MTGPlexer.TokenAnalysis;

/// <summary>
/// Enriched analysis of a single unique span of  text.
/// This object is now prepared at construction to be passed directly to JavaScript for visualization.
/// </summary>
public record AnalyzedSpan
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

    public Dictionary<string, DeterministicPalette> CardPalettes { get; init; } = [];

    [JsonIgnore]
    public int MaximalSpanOccurrenceCount { get; init; }
    
    [JsonIgnore]
    public int TotalOccurrenceCount { get; init; }
    
    [JsonIgnore]
    public int WordCount { get; init; }
    
    [JsonIgnore]
    public Dictionary<string, int> OccurrencesPerCard { get; init; }

    public string[] ContainingCards { get; init; }
    
    public AnalyzedSpan(
        string text,
        int maximalSpanOccurrenceCount,
        List<SubSpanContext> occurrences,
        List<AdjacencyNode> precedingAdjacencies,
        List<AdjacencyNode> followingAdjacencies)
    {
        // --- Standard Initializations ---
        Text = text;
        MaximalSpanOccurrenceCount = maximalSpanOccurrenceCount;
        PrecedingAdjacencies = precedingAdjacencies;
        FollowingAdjacencies = followingAdjacencies;
        WordCount = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
        TotalOccurrenceCount = occurrences.Count;

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

        OccurrencesPerCard = occurrences
            .GroupBy(x => x.OriginalOccurrence.Key.CardName)
            .OrderByDescending(x => x.Count())
            .ThenBy(x => x.Key)
            .ToDictionary(x => x.Key, x => x.Count());

        ContainingCards = OccurrencesPerCard.Select(x => x.Key).ToArray();
        var positionalPalette = DeterministicPalette.GetPositionalPalette(ContainingCards.Length);

        for (int i = 0; i < ContainingCards.Length; i++)
            CardPalettes[ContainingCards[i]] = positionalPalette[i];
    }

    public override string ToString() => $"'{Text}' (Total: {TotalOccurrenceCount} | Maximal: {MaximalSpanOccurrenceCount})";
}