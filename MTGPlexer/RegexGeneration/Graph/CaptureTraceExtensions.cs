namespace MTGPlexer.RegexGeneration.Graph;

/// <summary>
/// Rendering-oriented helpers over a hydrated <see cref="CaptureTrace"/> tree, shared by every
/// GUI surface (CardLinesPage, TypeRegexPage) that needs to walk a TokenUnit's capture hierarchy.
/// </summary>
public static class CaptureTraceExtensions
{
    /// <summary>
    /// All instances of every direct child capture (a child's siblings represent repeated/list
    /// captures of the same named group), flattened and ordered by their position in the source text.
    /// </summary>
    public static IEnumerable<CaptureTrace> ChildInstances(this CaptureTrace node) =>
        node.Children.SelectMany(child => child).OrderBy(c => c.Index);

    public static bool IsBranch(this CaptureTrace node) => node.Children.Count > 0;

    /// <summary>
    /// True when a node exists only to pass its entire span through to a single child (e.g. a OneOf
    /// wrapping its chosen alternative) without capturing any text of its own. Such nodes are skipped
    /// during rendering so they don't add a meaningless extra layer of nesting.
    /// </summary>
    public static bool IsCollapsedWrapper(this CaptureTrace node) =>
        node.Children.Count == 1
        && node.Children[0].Siblings.Count == 0
        && node.Children[0].Index == node.Index
        && node.Children[0].End == node.End;

    public static Type ClrType(this CaptureTrace node) => node.ClrValue?.GetType();

    /// <summary>
    /// A deterministic, content-addressed color for this capture's property name, consistent
    /// everywhere the same property is rendered (card lines, regex viewer, etc.).
    /// </summary>
    public static HexPalette Palette(this CaptureTrace node) =>
        string.IsNullOrEmpty(node.Name) ? null : new DeterministicPalette(node.Name).Palette;

    /// <summary>
    /// The deepest number of nested branch layers under this node, skipping collapsed wrappers.
    /// Used to reserve enough line-height to stack that many underlines.
    /// </summary>
    public static int GetMaxBranchDepth(this CaptureTrace node)
    {
        if (node.IsCollapsedWrapper())
            return node.ChildInstances().Select(GetMaxBranchDepth).DefaultIfEmpty(0).Max();

        if (!node.IsBranch())
            return 0;

        return 1 + node.ChildInstances().Select(GetMaxBranchDepth).DefaultIfEmpty(0).Max();
    }
}
