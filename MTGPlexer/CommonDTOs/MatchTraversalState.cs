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
    public Match RootMatch { get; }

    /// <summary>
    /// The fully-qualified path pointing to the capture at this instance's level in the tree hierarchy. Used to
    /// retrieve values from the RootMatch GroupCollection by fully-qualified name.
    /// </summary>
    public CaptureGroupPropPath CapturePath { get; }

    /// <summary>
    /// The ordinal capture position within the named Group that this match instance is found at. For named groups
    /// where the expected number of captures is 1 (which is most), this value is expected to be 0 and may be ignored.
    /// When non-zero (including when parent TokenUnitMatches are non-zero) this value is used to scope/hone which subsets
    /// of captures this instance is allowed to return given a fully-qualified capture group name. In other words, this
    /// value disambiguates which "capture path" to choose when a fully-qualified named group "node" contains multipled captures.
    /// </summary>
    public int CaptureOrdinal { get; init; }

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

    /// <summary>
    /// Constructor called by Tokenizer for top-level "root" matches with no TokenUnit parent.
    /// </summary>
    public MatchTraversalState(Type type, Match regexMatch)
    {
        ArgumentNullException.ThrowIfNull(regexMatch);

        Type = type;
        RootMatch = regexMatch;
        CapturePath = new(type.Name);
        AbsoluteEnd = RootMatch.Index + RootMatch.Length;
    }

    /// <summary>
    /// Constructor called by child properties to be hydrated within a parent "root" TokenUnit.
    /// </summary>
    public MatchTraversalState(Type type, MatchTraversalState parentTokenUnitMatch, string pathNameToAppend, int captureOrdinal = 0)
    {
        Type = type;
        Parent = parentTokenUnitMatch;
        RootMatch = parentTokenUnitMatch.RootMatch; // Always propagated from the parent
        CapturePath = parentTokenUnitMatch.CapturePath.Append(pathNameToAppend);
        CaptureOrdinal = captureOrdinal;
        AbsoluteEnd = parentTokenUnitMatch.AbsoluteEnd;
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
            if (groupLeafName == null)
                throw new ArgumentNullException(nameof(groupLeafName));

            var fullyQualifiedGroupName = CapturePath.GetFullyQualifiedNameFromLeaf(groupLeafName);

            if (RootMatch.Groups[fullyQualifiedGroupName].Success)
            {
                var capturePathScope = GetCapturePathScope();
                var allCapturesInGroup = RootMatch.Groups[fullyQualifiedGroupName].Captures;

                if (capturePathScope.PathIsConstrainedToScope)
                    // If path is constrained to some scope, return only those captures within the scope
                    return allCapturesInGroup
                        .Where(x => x.Index >= capturePathScope.Start && x.Index + x.Length <= capturePathScope.End)
                        .Select(x => new ExtractedCapture(x))
                        .ToArray();
                else
                    // Otherwise, return all the captures
                    return RootMatch.Groups[fullyQualifiedGroupName].Captures
                        .Select(x => new ExtractedCapture(x))
                        .ToArray();
            }

            // No group exists for the fully qualified name
            return [];
        }
    }

    CapturePathScope GetCapturePathScope()
    {
        MatchTraversalState currentNode = this;

        // Begining with self, search back through the parental lineage looking for the first branch choice, if any
        while (currentNode.Parent != null)
        {
            var currentNodeGroup = RootMatch.Groups[currentNode.CapturePath.FullyQualifiedCaptureGroupName];
            
            // Check whether this is a branching node
            if (currentNodeGroup.Captures.Count > 1)
            {
                // We've found the latest-extant branch, so return a scope constraint reflecting the current node's path choice
                var currentNodeBranch = currentNodeGroup.Captures[currentNode.CaptureOrdinal];
                var startOfScope = currentNodeBranch.Index;
                var endOfScope = currentNodeBranch.Index + currentNodeBranch.Length;

                return new(true, startOfScope, endOfScope);
            }

            currentNode = currentNode.Parent;
        }

        // There is no scope constraint
        return new();
    }

    public override string ToString() => $"Match: \"{RootMatch.Value}\"";
}

// Local helper record for tracking capture path scopes
record CapturePathScope(bool PathIsConstrainedToScope = false, int Start = -1, int End = -1);
