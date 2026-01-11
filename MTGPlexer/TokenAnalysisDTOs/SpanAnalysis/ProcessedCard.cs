namespace MTGPlexer.TokenAnalysisDTOs.SpanAnalysis;

using MTGPlexer.Colors;

/// <summary>
/// Represents a single card and all its processed lines of text.
/// This is a lightweight container for the results of the CorpusAnalyzer.
/// </summary>
public class ProcessedCard
{
    static HashSet<string> _irrelevantUnmatchedStrings = [".", ". ", " "];

    public Card Card { get; init; }
    public List<ProcessedLine> Lines { get; init; }
    public bool IsFullyMatched { get; init; }

    public ProcessedCard(Card card)
    {
        Card = card;
        Lines = ProcessedLine.GetAll(card);

        // "Fully matched" means no unmatched text occurrences (except isolated periods) exist
        IsFullyMatched = Lines
            .SelectMany(x => x.UnmatchedTextOccurrences.Where(y => !_irrelevantUnmatchedStrings.Contains(y.Text)))
            .Count() == 0;

        SetPositionalPalettes();
    }

    public void SetPositionalPalettes()
    {
        // 1. Collect all colorable nodes (Branches and Leaves) across all lines in DFS order
        var nodesToColor = new List<IHasPalette>();
        foreach (var line in Lines)
        {
            foreach (var root in line.SpanRoots)
            {
                CollectColorableNodes(root, nodesToColor);
            }
        }

        if (nodesToColor.Count == 0) return;

        // 2. Generate the rainbow segments once based on the total count
        var positionalPalettes = DeterministicPalette.GetPositionalPaletteSet(nodesToColor.Count);

        // 3. Assign the palettes back to the nodes
        for (int i = 0; i < nodesToColor.Count; i++)
        {
            var palette = positionalPalettes[i];
            var node = nodesToColor[i];
            node.Palette = palette;
        }
    }

    /// <summary>
    /// Recursively traverses the span tree to find all nodes that support palettes.
    /// </summary>
    private void CollectColorableNodes(SpanNode node, List<IHasPalette> collection)
    {
        // SpanRoot inherits from SpanBranch, and SpanSubLeaf inherits from SpanLeaf, 
        // so this covers all required types.
        if (node is IHasPalette nodeWithPalette && node.ElementType != TokenAnalysisElementType.UnmatchedTokenUnitRoot)
            collection.Add(nodeWithPalette);

        foreach (var child in node.Children)
            CollectColorableNodes(child, collection);
    }
}