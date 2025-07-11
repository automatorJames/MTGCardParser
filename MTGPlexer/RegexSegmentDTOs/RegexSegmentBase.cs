namespace MTGPlexer.RegexSegmentDTOs;

/// <summary>
/// The base of all regex segment types, including regex patterns not associated with any TokenUnit property,
/// as well as those associated with enum, bool, text placeholder, and child TokenUnit property types. Conceptually,
/// this is a segment of Regex within a broader RegexTemplate which combines with other segments into a finished
/// rendered Regex string & Regex object.
/// </summary>
public abstract record RegexSegmentBase
{
    public Regex Regex { get; protected set; }
    public string RegexString { get; protected set; }
}

