namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public record CaptureGroupPropPath
{
    public string PropPath { get; }
    public string TerminalProp { get; }
    public string PropPathFriendly { get; }
    public string TerminalPropFriendly { get; }

    public CaptureGroupPropPath(List<string> propPathParts)
    {
        PropPath = string.Join('.', propPathParts);
        PropPathFriendly = string.Join(": ", propPathParts.Skip(1).Select(x => x.ToFriendlyCase(TitleDisplayOption.Sentence))); // Skip the root type
        TerminalProp = propPathParts.LastOrDefault();
        TerminalPropFriendly = TerminalProp?.ToFriendlyCase(TitleDisplayOption.Sentence);
    }
}