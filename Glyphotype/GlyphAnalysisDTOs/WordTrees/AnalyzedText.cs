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

    /// <summary>All document names that contain this span, alphabetized - the order the key strip above the tree renders in.</summary>
    [JsonPropertyName("containingDocuments")]
    public string[] ContainingDocuments { get; init; }

    /// <summary>
    /// The friendly names of every top-level <see cref="Glyph"/> type that captured at least one
    /// stretch of text somewhere in this tree, alphabetized - the order the key strip below the tree
    /// renders in. Empty when nothing adjacent to this span was captured by any Glyph.
    /// </summary>
    [JsonPropertyName("containingGlyphTypes")]
    public string[] ContainingGlyphTypes { get; init; }

    /// <summary>
    /// Maps each name in <see cref="ContainingGlyphTypes"/> to its assigned palette. Drawn from the
    /// same equidistant-hue wheel as <see cref="DocumentPalettes"/> but brightened and desaturated
    /// (see <see cref="GlyphKeyPaletteKnobs"/>) so the two color signals stay tellable apart. Like
    /// the document palettes these are positional and therefore only meaningful within this one
    /// card - the same Glyph type gets a different hue in the next tree.
    /// </summary>
    [JsonPropertyName("glyphPalettes")]
    public Dictionary<string, HexPalette> GlyphPalettes { get; init; }

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

        // Alphabetical, not occurrence-ordered: hues here are positional and carry no meaning
        // outside this one card, so the only thing the order can usefully buy the reader is being
        // able to find a document name in the key.
        ContainingDocuments = OccurrencesPerDocument.Keys
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        DocumentPalettes = DeterministicPalette.GetPositionalPaletteSet(ContainingDocuments);

        ContainingGlyphTypes = CollectGlyphTypeNames();
        GlyphPalettes = BuildGlyphPalettes(ContainingGlyphTypes);

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

    /// <summary>
    /// The distinct glyph type names appearing anywhere in either adjacency tree, alphabetized.
    /// Null entries in a node's map mark uncaptured stretches and are skipped.
    /// </summary>
    string[] CollectGlyphTypeNames()
    {
        var names = new HashSet<string>(StringComparer.Ordinal);

        void Walk(IEnumerable<AdjacencyNode> nodes)
        {
            foreach (var node in nodes)
            {
                if (node.SpanGlyphTypes is { } glyphTypes)
                    foreach (var name in glyphTypes.Values)
                        if (name is not null)
                            names.Add(name);

                Walk(node.Children);
            }
        }

        Walk(PrecedingAdjacencies);
        Walk(FollowingAdjacencies);

        return names.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    static Dictionary<string, HexPalette> BuildGlyphPalettes(string[] glyphTypeNames) =>
        DeterministicPalette.GetPositionalPaletteSet(
            glyphTypeNames,
            GlyphKeyPaletteKnobs.SaturationFactor,
            GlyphKeyPaletteKnobs.LightnessFactor);

    public override string ToString() => $"'{Text}' (Total: {TotalOccurrenceCount} | Maximal: {MaximalSpanOccurrenceCount})";
}
