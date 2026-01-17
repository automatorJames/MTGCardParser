namespace MTGPlexer.CommonDTOs;

public record MatchTraversalState
{
    /// <summary>
    /// The type of root or property value this instance is meant to hydrate.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// The original top-level match that created this instance's root ancestor (or self if this instance is root).
    /// Contains all fully-qualified named capture groups required to hydrate all downstream child property values.
    /// The RootMatch the same for all descendents in the tree; it is not newly updated or scoped at each level.
    /// </summary>
    public ExtractedMatch RootMatch { get; }

    /// <summary>
    /// The fully-qualified path pointing to the capture at this instance's level in the tree hierarchy. Used to
    /// retrieve values from the RootMatch GroupCollection by fully-qualified name.
    /// </summary>
    public CaptureGroupPropPath CapturePath { get; }

    /// <summary>
    /// A convenience method that exposes the index of the end of this instance's match within the full original SourceText.
    /// Used to simplify scoping calculations, especially in the root-level Tokenizer.
    /// </summary>
    public int AbsoluteEnd { get; }

    /// <summary>
    /// The TokenUnitMatch "node" from which this child descended. Null for top-level "root" nodes. Used primarily to trace back
    /// the path to root and determine which nodes have branching capture paths where the correct path is determined by checking
    /// each node's CaptureOrdinal value.
    /// </summary>
    public MatchTraversalState Parent { get; }

    public ExtractedCapture Capture { get; }

    /// <summary>
    /// Constructor called by Tokenizer for top-level "root" matches with no TokenUnit parent.
    /// </summary>
    public MatchTraversalState(Type type, Match match)
    {
        ArgumentNullException.ThrowIfNull(match);

        Type = type;
        RootMatch = new(match);
        CapturePath = new(type.Name);
        AbsoluteEnd = match.Index + match.Length;
        Capture = new(match, "root");
    }

    /// <summary>
    /// Constructor called by child properties to be hydrated within a parent "root" TokenUnit.
    /// </summary>
    public MatchTraversalState(Type type, MatchTraversalState parentTokenUnitMatch, string pathNameToAppend, ExtractedCapture scopedCapture = null)
    {
        Type = type;
        Parent = parentTokenUnitMatch;
        RootMatch = parentTokenUnitMatch.RootMatch; // Always propagated from the parent
        CapturePath = parentTokenUnitMatch.CapturePath.Append(pathNameToAppend);
        AbsoluteEnd = parentTokenUnitMatch.AbsoluteEnd;
        Capture = scopedCapture ?? parentTokenUnitMatch[pathNameToAppend].FirstOrDefault();
    }

    /// <summary>
    /// Takes a group leaf name and constructs a fully qualified path using CapturePath. If a capture group
    /// exists in the RegexMatch by that name, its captures are returned. If any parent of this node is a branching
    /// point (i.e. its named group contains multiple captures), then we first find the latest such branch and only
    /// return the captures that fit the scope of that branch.
    /// </summary>
    public ExtractedCapture[] this[string groupLeafName]
    {
        get
        {
            var fullyQualifiedGroupName = CapturePath.GetFullyQualifiedNameFromLeaf(groupLeafName);
            return RootMatch[fullyQualifiedGroupName];

            //if (RootMatch.Groups[fullyQualifiedGroupName].Success)
            //{
            //    return RootMatch.Groups[fullyQualifiedGroupName].Captures
            //        .Select((x, idx) => new ExtractedCapture(x, fullyQualifiedGroupName, idx))
            //        .ToArray();
            //
            //    //var capturePathScope = GetCapturePathScope();
            //    //
            //    //if (fullyQualifiedGroupName.EndsWith("SecondPlus_Buff")) Debugger.Break();
            //    //
            //    //if (capturePathScope.PathIsConstrainedToScope)
            //    //    // If path is constrained to some scope, return only those captures within the scope
            //    //    return allCapturesInGroup
            //    //        .Where(x => x.Index >= capturePathScope.Start && x.Index + x.Length <= capturePathScope.End)
            //    //        .Select(x => new ExtractedCapture(x))
            //    //        .ToArray();
            //    //else
            //    //    // Otherwise, return all the captures
            //    //    return RootMatch.Groups[fullyQualifiedGroupName].Captures
            //    //        .Select(x => new ExtractedCapture(x))
            //    //        .ToArray();
            //}
            //
            //// No group exists for the fully qualified name
            //return [];
        }
    }

    //CapturePathScope GetCapturePathScope()
    //{
    //    MatchTraversalState currentNode = this;
    //
    //    // Begining with self, search back through the parental lineage looking for the first branch choice, if any
    //    while (currentNode.Parent != null)
    //    {
    //        var currentNodeGroup = RootMatch.Groups[currentNode.CapturePath.FullyQualifiedCaptureGroupName];
    //        
    //        // Check whether this is a branching node
    //        if (currentNodeGroup.Captures.Count > 1)
    //        {
    //            // We've found the latest-extant branch, so return a scope constraint reflecting the current node's path choice
    //            var currentNodeBranch = currentNodeGroup.Captures[currentNode.CaptureOrdinal];
    //            var startOfScope = currentNodeBranch.Index;
    //            var endOfScope = currentNodeBranch.Index + currentNodeBranch.Length;
    //
    //            return new(true, startOfScope, endOfScope);
    //        }
    //
    //        currentNode = currentNode.Parent;
    //    }
    //
    //    // There is no scope constraint
    //    return new();
    //}

    public ExtractedCapture GetScopedCapture(string leafName, MatchTraversalState context)
    {
        var fullyQualifiedGroupName = CapturePath.GetFullyQualifiedNameFromLeaf(leafName);
        var captures = RootMatch[fullyQualifiedGroupName];
        var singleCapture = captures.SingleOrDefault(x => x.Index >= context.Capture.Index && x.End <= context.Capture.End);

        return singleCapture;
    }

    public override string ToString() => $"Match: \"{RootMatch.Value}\"";
}

// Local helper record for tracking capture path scopes
record CapturePathScope(bool PathIsConstrainedToScope = false, int Start = -1, int End = -1);
