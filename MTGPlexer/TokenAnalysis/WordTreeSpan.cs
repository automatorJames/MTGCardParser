using MTGPlexer.TokenAnalysis.UnmatchedSpanDTOs;

namespace MTGPlexer.TokenAnalysis;

/// <summary>
/// A data transfer object representing the entire span visualization for JS.
/// </summary>
public record WordTreeSpan(
    string Id,
    string Text,
    List<WordTreeNode> Preceding,
    List<WordTreeNode> Following,
    List<List<string>> Sentences)
{
    public static WordTreeSpan FromAnalyzedSpan(AnalyzedUnmatchedSpan span)
    {
        var nodeIdCounter = 0;
        var nodeMap = new Dictionary<AdjacencyNode, string>();

        WordTreeNode ConvertNode(AdjacencyNode node)
        {
            if (!nodeMap.TryGetValue(node, out var nodeId))
            {
                nodeId = $"n{nodeIdCounter++}";
                nodeMap[node] = nodeId;
            }
            var tokenColor = node.TokenType is null ? null : TokenTypeRegistry.Palettes[node.TokenType].Hex;
            return new WordTreeNode(nodeId, node.Text, tokenColor, node.Children.Select(ConvertNode).ToList());
        }

        var precedingNodes = span.PrecedingAdjacencies.Select(ConvertNode).ToList();
        var followingNodes = span.FollowingAdjacencies.Select(ConvertNode).ToList();

        // --- FIX: Robust Sentence Path Reconstruction ---
        var sentences = new List<List<string>>();
        var leftLeaves = FindLeaves(span.PrecedingAdjacencies);
        var rightLeaves = FindLeaves(span.FollowingAdjacencies);

        if (leftLeaves.Any() && rightLeaves.Any())
        {
            // Case 1: Nodes on both sides (original logic)
            foreach (var leftLeaf in leftLeaves)
            {
                var leftPath = GetPathToRoot(span.PrecedingAdjacencies, leftLeaf, nodeMap);
                foreach (var rightLeaf in rightLeaves)
                {
                    var rightPath = GetPathToRoot(span.FollowingAdjacencies, rightLeaf, nodeMap);
                    sentences.Add(leftPath.Concat(rightPath).ToList());
                }
            }
        }
        else if (leftLeaves.Any())
        {
            // Case 2: Only preceding nodes exist
            foreach (var leftLeaf in leftLeaves)
            {
                sentences.Add(GetPathToRoot(span.PrecedingAdjacencies, leftLeaf, nodeMap).ToList());
            }
        }
        else if (rightLeaves.Any())
        {
            // Case 3: Only following nodes exist
            foreach (var rightLeaf in rightLeaves)
            {
                sentences.Add(GetPathToRoot(span.FollowingAdjacencies, rightLeaf, nodeMap).ToList());
            }
        }

        return new WordTreeSpan($"span-{Guid.NewGuid()}", span.Text, precedingNodes, followingNodes, sentences);
    }

    private static List<AdjacencyNode> FindLeaves(List<AdjacencyNode> forest)
    {
        var leaves = new List<AdjacencyNode>();
        Action<AdjacencyNode> find = null!;
        find = (node) => {
            if (node.Children.Any()) node.Children.ForEach(find);
            else leaves.Add(node);
        };
        forest.ForEach(find);
        return leaves;
    }

    private static IEnumerable<string> GetPathToRoot(List<AdjacencyNode> forest, AdjacencyNode leaf, Dictionary<AdjacencyNode, string> nodeMap)
    {
        var path = new List<AdjacencyNode>();
        Func<AdjacencyNode, bool> findPath = null!;
        findPath = (current) => {
            path.Add(current);
            if (current == leaf) return true;
            if (current.Children.Any(child => findPath(child))) return true;
            path.RemoveAt(path.Count - 1);
            return false;
        };
        forest.Any(root => findPath(root));
        return path.Select(p => nodeMap[p]);
    }
}