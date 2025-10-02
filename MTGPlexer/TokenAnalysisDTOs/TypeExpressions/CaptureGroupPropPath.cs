namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public record CaptureGroupPropPath
{
    public string PropPath { get; }
    public string PropPathRelativeToRoot { get; }
    public string PropPathFriendly { get; }


    public CaptureGroupPropPath(string propPathIncludingRoot)
    {
        PropPath = propPathIncludingRoot;

        if (string.IsNullOrEmpty(propPathIncludingRoot))
            return;

        var splitPath = propPathIncludingRoot.Split('.');

        if (PropPath.Length >= 1)
        {
            PropPathRelativeToRoot = string.Join('.', splitPath);
            PropPathFriendly = string.Join('.', splitPath.Select(x => x.ToFriendlyCase(TitleDisplayOption.Sentence)));
        }
    }
}