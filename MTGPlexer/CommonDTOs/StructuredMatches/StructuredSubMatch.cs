namespace MTGPlexer.CommonDTOs.StructuredMatches;

public record StructuredSubMatch : StructuredPropMatch
{
    public Match SubMatch { get; }

    public StructuredSubMatch(Match subMatch, StructuredMatchBase parent) : base(parent, subMatch)
    {
        SubMatch = subMatch;
    }
}

