namespace MTGPlexer.TokenAnalysisDTOs.SpanAnalysis;

/// <summary>
/// Base record for all analysis nodes. Handles spatial properties and nested capture logic.
/// </summary>
public abstract record SpanNode
{
    public string Name { get; init; } = string.Empty;
    public CaptureGroupPropPath CapturePath { get; init; } = null!;
    public string CaptureTextOriginal { get; init; } = string.Empty;
    public int Start { get; init; }
    public int End { get; init; }
    public int Length { get; init; }
    public TokenAnalysisElementType ElementType { get; init; }
    public List<SpanNode> Children { get; init; } = new();

    /// <summary>
    /// Recursively builds the string representation of nested captures.
    /// </summary>
    protected string GetNestedCaptureString()
    {
        if (Children.Count == 0 || string.IsNullOrEmpty(CaptureTextOriginal))
            return CaptureTextOriginal;

        var builder = new StringBuilder(CaptureTextOriginal);

        foreach (var child in Children.OrderByDescending(c => c.Start))
        {
            if (child.Start < this.Start || child.End > this.End || child.Length == 0)
                continue;

            string content = child.GetNestedCaptureString();
            bool shouldWrap = child is not SpanBranch { IsCollapsed: true };
            string replacement = shouldWrap ? $"({content})" : content;

            int relativeStart = child.Start - this.Start;
            builder.Remove(relativeStart, child.Length);
            builder.Insert(relativeStart, replacement);
        }

        return builder.ToString();
    }
}