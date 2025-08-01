namespace MTGPlexer.TokenAnalysis;

/// <summary>
/// A data transfer object representing a single node for the JS word tree.
/// </summary>
public record WordTreeNode(string Id, string Text, string TokenTypeColor, List<WordTreeNode> Children);

