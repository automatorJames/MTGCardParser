using MTGPlexer.RegexGeneration.Graph.Nodes;

namespace MTGPlexer.RegexGeneration.Graph;

public class HydratedNodeGraph : RootNode
{
    public CaptureContext CaptureContext { get; }
    public string Value => CaptureContext.FullMatch;

    public HydratedNodeGraph(Type type, Match match, string sourceText) : base(type)
    {
        CaptureContext = CaptureContext.Create(match, sourceText);
    }

    public TokenUnit Hydrate()
    {
        var instance = (TokenUnit)Activator.CreateInstance(RootType);

        foreach (var captureChild in CaptureChildren)
        {
            // will return false only if an underlying property has AbortIfSetPropertyToNull == true
            // and the property value is null
            var setSuccessfully = captureChild.SetPropertyValue(CaptureContext, instance);

            if (!setSuccessfully)
                return null;
        }

        instance.NodeGraph = this;

        return instance;
    }

    /// <summary>
    /// Calculates the maximum depth of the node hierarchy.
    /// Collapsible nodes (like WrappedNodes) do not increment the depth count.
    /// </summary>
    public int GetRecursiveDepth()
    {
        return GetMaxDepth(this, 0);

        static int GetMaxDepth(RegexNode node, int currentDepth)
        {
            if (node is not BranchNode branch)
                return currentDepth;

            int maxFound = currentDepth;

            foreach (var child in branch.Children)
            {
                // TextNodes are structural literals and don't count toward logical data depth
                if (child is TextNode)
                    continue;

                // Increment depth only if the node is NOT collapsible.
                // WrappedNodes return IsCollapsible = true, so they act as pass-throughs.
                int childDepth = currentDepth + (child.IsCollapsible ? 0 : 1);

                int branchMax = GetMaxDepth(child, childDepth);
                if (branchMax > maxFound)
                {
                    maxFound = branchMax;
                }
            }

            return maxFound;
        }
    }
}