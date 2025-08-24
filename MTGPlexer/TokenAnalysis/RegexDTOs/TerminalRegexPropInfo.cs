namespace MTGPlexer.TokenAnalysis.RegexDTOs;

public record TerminalRegexPropPath
{
    public string TerminalPropName { get; }
    public string PropPathFriendly { get; }
    public RegexPropType RegexPropType { get; }
    public string RegexPropTypeNameFriendly { get; }

    public TerminalRegexPropPath(RegexPropInfo terminalPropInfo, IEnumerable<string> pathToTerminal)
    {
        TerminalPropName = pathToTerminal.Last();
        PropPathFriendly = string.Join(": ", pathToTerminal.Select(x => x.ToFriendlyCase(TitleDisplayOption.Title)));
        RegexPropType = terminalPropInfo.RegexPropType;
        RegexPropTypeNameFriendly = terminalPropInfo.FriendlyTypeName;
    }
}

