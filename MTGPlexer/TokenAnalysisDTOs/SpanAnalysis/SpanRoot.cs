namespace MTGPlexer.TokenAnalysisDTOs.SpanAnalysis;

/// <summary>
/// The specialized root of the analysis tree containing global context.
/// </summary>
public record SpanRoot : SpanBranch
{
    public string OriginalFullText { get; init; } = string.Empty;
    public TokenUnit RootToken { get; init; } = null!;
    public string CardName { get; init; } = string.Empty;
    public int ClauseIndex { get; init; }

    public int GetRecursiveDepth()
    {
        return GetMaxDepth(this, 0);

        static int GetMaxDepth(SpanNode node, int currentDepth)
        {
            int maxFound = currentDepth;

            foreach (var child in node.Children)
            {
                // Skip sub-leaves per the UI logic
                if (child is SpanSubLeaf) continue;

                // Only increment depth if the child is NOT a collapsed branch
                int childDepth = currentDepth + (child is SpanBranch { IsCollapsed: true } ? 0 : 1);

                int branchMax = GetMaxDepth(child, childDepth);
                if (branchMax > maxFound)
                {
                    maxFound = branchMax;
                }
            }

            return maxFound;
        }
    }

    public override string ToString()
    {
        return $"{CapturePath} | \"{GetNestedCaptureString()}\" | {ElementType} | Card: {CardName} | Children: {Children.Count}";
    }
}