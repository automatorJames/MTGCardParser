namespace MTGPlexer.TokenAnalysisDTOs.SpanAnalysis;

/// <summary>
/// Base record with properties common to all analysis nodes.
/// </summary>
public abstract record SpanAnalysisBase
{
    public string Name { get; init; }
    public CaptureGroupPropPath CapturePath { get; init; }
    public string CaptureTextOriginal { get; init; }
    public int Start { get; init; }
    public int End { get; init; }
    public int Length { get; init; }
    public TokenAnalysisElementType ElementType { get; init; }
    public IReadOnlyList<SpanAnalysisBase> Children { get; init; } = [];

    /// <summary>
    /// Recursively builds a string that shows the nesting of captures within the current summary's text.
    /// A collapsed branch will not add parentheses, making it invisible in the nested structure.
    /// Example: "The ((dog) runs fast)"
    /// </summary>
    protected string GetNestedCaptureString()
    {
        if (Children.Count == 0 || string.IsNullOrEmpty(CaptureTextOriginal))
            return CaptureTextOriginal;

        var builder = new StringBuilder(CaptureTextOriginal);

        // Process children from last to first to avoid invalidating indices.
        foreach (var child in Children.OrderByDescending(c => c.Start))
        {
            // Only process children that have a distinct sub-capture within this parent.
            if (child.Start < this.Start || child.End > this.End || child.Length == 0)
                continue;

            // Get the child's own nested string first.
            string childNestedString = child.GetNestedCaptureString();

            // A collapsed branch is invisible; it should not add parentheses.
            // Only wrap non-collapsed branches and all leaf types.
            bool shouldWrapInParentheses = true;
            if (child is SpanBranch branch && branch.IsCollapsed)
            {
                shouldWrapInParentheses = false;
            }

            string replacement = shouldWrapInParentheses
                ? $"({childNestedString})"
                : childNestedString;

            int relativeStart = child.Start - this.Start;

            builder.Remove(relativeStart, child.Length);
            builder.Insert(relativeStart, replacement);
        }

        return builder.ToString();
    }
}