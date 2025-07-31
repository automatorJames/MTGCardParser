namespace MTGPlexer.TokenAnalysis;

/// <summary>
/// Represents a single node in an adjacency tree. Each node is an item (a token or a word)
/// that can precede or follow a span, and it contains its own children, representing the items
/// that can follow it, forming a chain of possibilities.
/// </summary>
/// <param name="Text">The text of the item at this node (e.g., "faces" or "with glee").</param>
/// <param name="TokenType">The Type of the token if it was a matched token; otherwise, null.</param>
/// <param name="Frequency">How many times this specific path was taken in the corpus.</param>
/// <param name="Children">A list of further nodes that follow this one, forming the next level of the tree.</param>
public record AdjacencyNode(string Text, Type TokenType, int Frequency, List<AdjacencyNode> Children);
