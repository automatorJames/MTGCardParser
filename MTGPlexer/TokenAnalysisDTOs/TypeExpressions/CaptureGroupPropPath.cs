namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public record CaptureGroupPropPath
{
    public string LeafName { get; }
    public string PropPath { get; }
    public string PropPathRelativeToRoot { get; }
    public string PropPathFriendly { get; }
    public CaptureGroupPropPath Parent { get; init; }

    public CaptureGroupPropPath(string propPathIncludingRoot)
    {
        PropPath = propPathIncludingRoot;

        if (string.IsNullOrEmpty(propPathIncludingRoot))
            return;

        var splitPath = propPathIncludingRoot.Split('.');

        if (PropPath.Length > 1)
        {
            var pathPartsRelativeToRoot = splitPath.Skip(1);

            PropPathRelativeToRoot = string.Join('.', pathPartsRelativeToRoot);
            PropPathFriendly = string.Join(": ", pathPartsRelativeToRoot.Select(x => x.ToFriendlyCase(TitleDisplayOption.Sentence)));
            Parent = new(string.Join('.', splitPath.Take(splitPath.Length - 1)));
        }

        LeafName = splitPath.Last();
    }

    public CaptureGroupPropPath Append(string partToAppend)
    {
        return new(PropPath.Dot(partToAppend))
        {
            Parent = this
        };
    }

    public override string ToString() => PropPath;
}