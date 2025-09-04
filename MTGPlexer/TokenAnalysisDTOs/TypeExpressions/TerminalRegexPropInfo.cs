namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public record TerminalRegexPropPath
{
    public string TerminalPropName { get; }
    public string PropPathFriendly { get; }
    public RegexPropInfo Prop { get; }
    public string RegexPropTypeNameFriendly { get; }

    public TerminalRegexPropPath(RegexPropInfo terminalPropInfo, IEnumerable<string> pathToTerminal)
    {
        TerminalPropName = pathToTerminal.Last();
        PropPathFriendly = string.Join(": ", pathToTerminal.Select(x => x.ToFriendlyCase(TitleDisplayOption.Title)));
        Prop = terminalPropInfo;
        RegexPropTypeNameFriendly = terminalPropInfo.FriendlyTypeName;
    }
}

