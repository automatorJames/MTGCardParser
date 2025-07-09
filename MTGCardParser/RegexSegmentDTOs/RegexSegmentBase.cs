namespace MTGCardParser.RegexSegmentDTOs;

public abstract record RegexSegmentBase : IRegexSegment
{
    public Regex Regex { get; protected set; }
    public string RegexString { get; protected set; }
}

