namespace MTGPlexer.TokenAnalysisDTOs.SpanAnalysis;

/// <summary>
/// A branch node that groups other nodes. Encapsulates palette and collapse logic.
/// </summary>
public record SpanBranch : SpanNode, IHasPalette
{
    public HexPalette Palette { get; set; }
    public bool IsCollapsed { get; init; }

    /// <summary>
    /// Determines if this node should be visually collapsed. 
    /// A node is collapsed if all of its children are also branches.
    /// </summary>
    public static bool CalculateIsCollapsed(IEnumerable<SpanNode> children)
    {
        var list = children.ToList();
        return list.Count > 0 && list.All(c => c is SpanBranch);
    }

    public override string ToString()
    {
        string nested = GetNestedCaptureString();
        return $"{CapturePath} | [{Start}] \"{nested}\" [{End}] | {ElementType} | Collapsed: {IsCollapsed} | Children: {Children.Count}";
    }
}