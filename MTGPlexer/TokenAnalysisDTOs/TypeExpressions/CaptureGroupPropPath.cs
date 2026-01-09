namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public record CaptureGroupPropPath
{
    public string LeafName { get; }
    public string PropPath { get; }
    public string PropPathRelativeToRoot { get; }
    public string PropPathFriendly { get; }
    public CaptureGroupPropPath Parent { get; }
    public string FullyQualifiedCaptureGroupName { get; }

    public CaptureGroupPropPath(string propPathIncludingRoot)
    {
        PropPath = propPathIncludingRoot;

        if (string.IsNullOrEmpty(propPathIncludingRoot))
            return;

        var propPathPartsWithRoot = propPathIncludingRoot.Split('.');

        if (propPathPartsWithRoot.Length > 1)
        {
            var propPathPartsRelativeToRoot = propPathPartsWithRoot.Skip(1).ToArray();
            PropPathRelativeToRoot = string.Join('.', propPathPartsRelativeToRoot);
            PropPathFriendly = string.Join(": ", propPathPartsRelativeToRoot.Select(x => x.ToFriendlyCase(TitleDisplayOption.Sentence)));
            Parent = new(string.Join('.', propPathPartsWithRoot.Take(propPathPartsWithRoot.Length - 1)));
            FullyQualifiedCaptureGroupName = string.Join('_', propPathPartsRelativeToRoot);
        }

        LeafName = propPathPartsWithRoot.Last();
    }

    public CaptureGroupPropPath Append(params string[] partsToAppend) => new(PropPath.Dot(partsToAppend));

    public string GetFullyQualifiedNameFromLeaf(string leafName)
    {
        if (FullyQualifiedCaptureGroupName == null)
            return leafName;

        return FullyQualifiedCaptureGroupName + "_" + leafName;
    }

    public override string ToString() => PropPath;
}