namespace MTGPlexer.CommonDTOs;

public record TokenUnitMatch
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
    public int CaptureOrdinal { get; }

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
    public TokenUnitMatch Parent { get; }

    /// <summary>
    /// Constructor called by Tokenizer for top-level "root" matches with no TokenUnit parent.
    /// </summary>
    public TokenUnitMatch(Type type, Match regexMatch)
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
    public TokenUnitMatch(Type type, TokenUnit parentTokenUnit, string pathNameToAppend, int captureOrdinal = 0)
    {
        Type = type;
        RootMatch = parentTokenUnit.Match.RootMatch; // Always propagated from the parent
        CapturePath = parentTokenUnit.Match.CapturePath.Append(pathNameToAppend);
        CaptureOrdinal = captureOrdinal;
    }

    /// <summary>
    /// Takes a group leaf name and constructs a fully qualified path using CapturePath. If a capture group
    /// exists in the RegexMatch by that name it is returned. If a capture ordinal is provided, this indexer
    /// validates that the named group contgains at least as many captures as the ordinal position (note: it 
    /// does not isolate and return the capture at this position, but rather the containing group).
    /// </summary>
    public Group this[string groupLeafName, int? captureOrdinal = null]
    {
        get
        {
            if (groupLeafName == null)
                throw new ArgumentNullException(nameof(groupLeafName));

            if (captureOrdinal != null && captureOrdinal.Value < 0)
                throw new ArgumentOutOfRangeException(nameof(captureOrdinal));

            var fullyQualifiedGroupName = CapturePath.GetFullyQualifiedNameFromLeaf(groupLeafName);

            if (RootMatch.Groups[fullyQualifiedGroupName].Success)
            {
                // If a capture ordinal is provided, the group must contain at least that many captures, else we return null
                if (captureOrdinal.HasValue && RootMatch.Groups[fullyQualifiedGroupName].Captures.Count - 1 < captureOrdinal)
                    return null;

                // Otherwise, return the named group
                return RootMatch.Groups[fullyQualifiedGroupName];
            }

            // No group exists for the fully qualified name
            return null;
        }
    }

    public override string ToString() => $"Match: \"{RootMatch.Value}\"";
}
