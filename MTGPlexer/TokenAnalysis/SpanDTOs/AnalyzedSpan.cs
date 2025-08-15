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

    // --- Ignored Properties (Server-Side Only) ---

    [JsonIgnore]
    public int MaximalSpanOccurrenceCount { get; init; }

    [JsonIgnore]
    public int TotalOccurrenceCount { get; init; }

    [JsonIgnore]
    public int WordCount { get; init; }

    [JsonIgnore]
    public Dictionary<string, int> OccurrencesPerCard { get; init; }

    /// <summary>
    /// Gets a distinct list of all palettes that have a non-null Seed (i.e., they represent a token type)
    /// found within any node in the preceding or following adjacency trees. This is used by the
    /// frontend to render the list of token types found in the tree.
    /// </summary>
    [JsonIgnore]
    public List<DeterministicPalette> DistinctSeedPalettes
    {
        get
        {
            var palettes = new Dictionary<string, DeterministicPalette>();

            void Traverse(IEnumerable<AdjacencyNode> nodes)
            {
                if (nodes == null) return;
                foreach (var node in nodes)
                {
                    if (node.Segment?.Palettes != null)
                    {
                        foreach (var p in node.Segment.Palettes.Values)
                        {
                            // We only care about palettes with a seed, which identifies them as a token type.
                            // We use a dictionary to ensure each seed is represented only once.
                            if (p?.Seed != null && !palettes.ContainsKey(p.Seed))
                            {
                                palettes[p.Seed] = p;
                            }
                        }
                    }
                    Traverse(node.Children);
                }
            }

            Traverse(PrecedingAdjacencies);
            Traverse(FollowingAdjacencies);

            return palettes.Values.OrderBy(p => p.Seed).ToList();
        }
    }

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