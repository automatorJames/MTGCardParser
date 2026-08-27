namespace Glyphotype.GlyphAnalysisDTOs.WordTrees;

/// <summary>
/// Enriched analysis of a single unique span of text.
/// This object is now prepared at construction to be passed directly to JavaScript for visualization.
/// </summary>
public record AnalyzedText
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

    /// <summary>Maps a document name to its assigned color palette.</summary>
    [JsonPropertyName("documentPalettes")]
    public Dictionary<string, HexPalette> DocumentPalettes { get; init; }

    /// <summary>An array of all document names that contain this span, ordered for the UI.</summary>
    [JsonPropertyName("containingDocuments")]
    public string[] ContainingDocuments { get; init; }

    // --- Ignored Properties (Server-Side Only) ---

    [JsonIgnore]
    public int MaximalSpanOccurrenceCount { get; init; }

    [JsonIgnore]
    public int TotalOccurrenceCount { get; init; }

    [JsonIgnore]
    public int WordCount { get; init; }

    [JsonIgnore]
    public Dictionary<string, int> OccurrencesPerDocument { get; init; }

    /// <summary>
    /// True unless every occurrence of this (already right-maximal) span shares one identical
    /// immediately-preceding word — i.e. this span is not simply the tail end of some longer,
    /// equally-frequent phrase. A span reaching the start of its source occurrence (no preceding
    /// word at all) always counts as left-maximal, mirroring how reaching the end of an occurrence
    /// is treated as maximal on the right.
    /// </summary>
    [JsonIgnore]
    public bool IsLeftMaximal { get; init; }

    public AnalyzedText(
        string text,
        int maximalSpanOccurrenceCount,
        List<SubSpanContext> occurrences,
        List<AdjacencyNode> precedingAdjacencies,
        List<AdjacencyNode> followingAdjacencies,
        bool isLeftMaximal)
    {
        // --- Standard Initializations ---
        Text = text;
        MaximalSpanOccurrenceCount = maximalSpanOccurrenceCount;
        PrecedingAdjacencies = precedingAdjacencies;
        FollowingAdjacencies = followingAdjacencies;
        IsLeftMaximal = isLeftMaximal;
        WordCount = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
        TotalOccurrenceCount = occurrences.Count;

        OccurrencesPerDocument = occurrences
            .GroupBy(x => x.OriginalOccurrence.DocumentName)
            .OrderByDescending(x => x.Count())
            .ThenBy(x => x.Key)
            .ToDictionary(x => x.Key, x => x.Count());

        ContainingDocuments = OccurrencesPerDocument.Keys.ToArray();
        var positionalPalette = DeterministicPalette.GetPositionalPaletteSet(ContainingDocuments.Length);

        DocumentPalettes = new Dictionary<string, HexPalette>();
        for (int i = 0; i < ContainingDocuments.Length; i++)
            DocumentPalettes[ContainingDocuments[i]] = positionalPalette[i];

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