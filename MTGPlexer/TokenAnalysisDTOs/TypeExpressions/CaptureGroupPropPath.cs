namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public record CaptureGroupPropPath
{
    public string PropPath { get; }
    public string PropPathRelativeToRoot { get; }
    public string PropPathFriendly { get; }

    public CaptureGroupPropPath(List<string> propPathParts)
    {
        PropPath = string.Join('.', propPathParts);
        PropPathRelativeToRoot = string.Join(".", propPathParts.Skip(1)); // Skip the root type
        PropPathFriendly = string.Join(": ", propPathParts.Skip(1).Select(x => x.ToFriendlyCase(TitleDisplayOption.Sentence))); // Skip the root type
    }
}