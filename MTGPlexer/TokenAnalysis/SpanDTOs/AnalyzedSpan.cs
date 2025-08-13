namespace MTGPlexer.TokenAnalysis.SpanDTOs;

/// <summary>
/// Enriched analysis of a single unique span of text.
/// This object is now prepared at construction to be passed directly to JavaScript for visualization.
/// </summary>
public record AnalyzedSpan
{
    // --- Core Data Properties ---

    /// <summary>The text of the span. This serves as the anchor text in the visualization.</summary>
    [JsonPropertyName("text")]
    public string Text { get; init; }

    /// <summary>A hierarchical tree representing all token/word sequences that appeared immediately BEFORE this span.</summary>
    [JsonPropertyName("precedingAdjacencies")]
    public List<AdjacencyNode> PrecedingAdjacencies { get; init; }

    /// <summary>A hierarchical tree representing all token/word sequences that appeared immediately AFTER this span.</summary>
    [JsonPropertyName("followingAdjacencies")]
    public List<AdjacencyNode> FollowingAdjacencies { get; init; }

    /// <summary>Maps a card name to its assigned color palette.</summary>
    [JsonPropertyName("cardPalettes")]
    public Dictionary<string, DeterministicPalette> CardPalettes { get; init; }

    /// <summary>An array of all card names that contain this span, ordered for the UI.</summary>
    [JsonPropertyName("containingCards")]
    public string[] ContainingCards { get; init; }

    // --- REMOVED: Pre-calculated Lookups for JavaScript ---
    // These are no longer needed as the simplified key (CardName) makes
    // them redundant with CardPalettes and ContainingCards.

    // --- Ignored Properties (Server-Side Only) ---

    [JsonIgnore]
    public int MaximalSpanOccurrenceCount { get; init; }

    [JsonIgnore]
    public int TotalOccurrenceCount { get; init; }

    [JsonIgnore]
    public int WordCount { get; init; }

    [JsonIgnore]
    public Dictionary<string, int> OccurrencesPerCard { get; init; }

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

        OccurrencesPerCard = occurrences
            .GroupBy(x => x.OriginalOccurrence.Key.CardName)
            .OrderByDescending(x => x.Count())
            .ThenBy(x => x.Key)
            .ToDictionary(x => x.Key, x => x.Count());

        ContainingCards = OccurrencesPerCard.Keys.ToArray();
        var positionalPalette = DeterministicPalette.GetPositionalPalette(ContainingCards.Length);

        CardPalettes = new Dictionary<string, DeterministicPalette>();
        for (int i = 0; i < ContainingCards.Length; i++)
            CardPalettes[ContainingCards[i]] = positionalPalette[i];

        // --- HYDRATION STEP ---
        int nodeIdCounter = 0;
        void TraverseAndHydrateIds(IEnumerable<AdjacencyNode> nodes)
        {
            foreach (var node in nodes)
            {
                node.Id = $"n{nodeIdCounter++}"; // Hydrate with unique ID
                TraverseAndHydrateIds(node.Children);
            }
        }

        TraverseAndHydrateIds(PrecedingAdjacencies);
        TraverseAndHydrateIds(FollowingAdjacencies);
    }

    public override string ToString() => $"'{Text}' (Total: {TotalOccurrenceCount} | Maximal: {MaximalSpanOccurrenceCount})";
}