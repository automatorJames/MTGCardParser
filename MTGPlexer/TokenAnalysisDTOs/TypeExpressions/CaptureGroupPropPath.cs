namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public record CaptureGroupPropPath
{
    public string LeafName { get; }
    public string PropPath { get; }
    public string PropPathRelativeToRoot { get; }
    public string PropPathFriendly { get; }
    public string[] PropPathPartsRelativeToRoot { get; }
    public CaptureGroupPropPath Parent { get; init; }

    public CaptureGroupPropPath(string propPathIncludingRoot)
    {
        PropPath = propPathIncludingRoot;

        if (string.IsNullOrEmpty(propPathIncludingRoot))
            return;

        var propPathPartsWithRoot = propPathIncludingRoot.Split('.');

        if (PropPath.Length > 1)
        {
            PropPathPartsRelativeToRoot = propPathPartsWithRoot.Skip(1).ToArray();
            PropPathRelativeToRoot = string.Join('.', PropPathPartsRelativeToRoot);
            PropPathFriendly = string.Join(": ", PropPathPartsRelativeToRoot.Select(x => x.ToFriendlyCase(TitleDisplayOption.Sentence)));
            Parent = new(string.Join('.', PropPathPartsRelativeToRoot.Take(PropPathPartsRelativeToRoot.Length - 1)));
        }

        LeafName = propPathPartsWithRoot.Last();
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