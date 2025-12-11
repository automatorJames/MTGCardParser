namespace MTGPlexer.Analysis;

/// <summary>
/// The top-level container (The "Sidecar") attached to a TokenUnit instance.
/// Holds context about the card/line and the root of the analysis tree.
/// </summary>
public class TokenAnalysisSidecar
{
    public string CardName { get; }
    public int LineIndex { get; }
    public string FullOriginalLineText { get; }
    public TokenAnalysisNode Root { get; }
    public Type RootType { get; }

    public TokenAnalysisSidecar(string cardName, int lineIndex, string fullText, TokenUnit rootTokenUnit)
    {
        CardName = cardName;
        LineIndex = lineIndex;
        FullOriginalLineText = fullText;
        RootType = rootTokenUnit.GetType();

        // The builder logic is encapsulated here or in a factory
        Root = TokenAnalysisBuilder.BuildGraph(rootTokenUnit, fullText, cardName, lineIndex);
    }

    /// <summary>
    /// Returns a flat list of all nodes, useful for TypeExpression counting/aggregation.
    /// </summary>
    public IEnumerable<TokenAnalysisNode> GetAllNodes() => Root.DescendantsAndSelf();

    /// <summary>
    /// Returns only terminal values (Leaves) for quick value analysis.
    /// </summary>
    public IEnumerable<TokenAnalysisNode> GetTerminals() => Root.DescendantsAndSelf().Where(n => n.IsTerminal);
}