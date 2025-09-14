namespace MTGPlexer.CommonDTOs.StructuredMatches;

public record StructuredSubMatch : StructuredPropMatch
{
    public Match SubMatch { get; }

    public StructuredSubMatch(Match subMatch, StructuredMatchBase parent, RegexPropInfo regexPropInfo) : base(parent, subMatch, regexPropInfo)
    {
        SubMatch = subMatch;
    }
}

