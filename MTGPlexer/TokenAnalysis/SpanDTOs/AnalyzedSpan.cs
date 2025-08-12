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

    // --- NEW: Pre-calculated Lookups for JavaScript ---

    /// <summary>
    /// A flat map that directly associates a unique source occurrence key (e.g., "CardName[0:5]")
    /// with its corresponding color palette. This prevents the client from needing to derive this.
    /// </summary>
    [JsonPropertyName("keyToPaletteMap")]
    public Dictionary<string, DeterministicPalette> KeyToPaletteMap { get; init; }

    /// <summary>
    /// A simple array of all unique source occurrence keys found in the entire tree structure.
    /// This is used by the client for certain join operations.
    /// </summary>
    [JsonPropertyName("allKeys")]
    public List<string> AllKeys { get; init; }

    /// <summary>
    /// Maps a card name (e.g., "Sol Ring") to a list of all its specific occurrence keys
    /// (e.g., "Sol Ring[3:4]", "Sol Ring[10:11]"). This enables fast client-side highlighting.
    /// </summary>
    [JsonPropertyName("cardNameToKeysMap")]
    public Dictionary<string, List<string>> CardNameToKeysMap { get; init; }

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

        // --- HYDRATION & PRE-CALCULATION STEP ---
        var keyToPaletteMapBuilder = new Dictionary<string, DeterministicPalette>();
        var allKeysBuilder = new HashSet<string>();
        var cardNameToKeysMapBuilder = new Dictionary<string, List<string>>();
        int nodeIdCounter = 0;

        void TraverseAndBuildLookups(IEnumerable<AdjacencyNode> nodes)
        {
            foreach (var node in nodes)
            {
                node.Id = $"n{nodeIdCounter++}"; // Hydrate with unique ID

                foreach (var key in node.SourceOccurrenceKeys)
                {
                    allKeysBuilder.Add(key);
                    var cardName = key[..key.IndexOf('[')];

                    if (CardPalettes.TryGetValue(cardName, out var palette))
                    {
                        keyToPaletteMapBuilder[key] = palette;
                    }

                    if (!cardNameToKeysMapBuilder.ContainsKey(cardName))
                    {
                        cardNameToKeysMapBuilder[cardName] = new List<string>();
                    }
                    cardNameToKeysMapBuilder[cardName].Add(key);
                }
                TraverseAndBuildLookups(node.Children);
            }
        }

        TraverseAndBuildLookups(PrecedingAdjacencies);
        TraverseAndBuildLookups(FollowingAdjacencies);

        // Finalize the public properties
        KeyToPaletteMap = keyToPaletteMapBuilder;
        AllKeys = allKeysBuilder.ToList();
        CardNameToKeysMap = cardNameToKeysMapBuilder;
    }

    public override string ToString() => $"'{Text}' (Total: {TotalOccurrenceCount} | Maximal: {MaximalSpanOccurrenceCount})";
}