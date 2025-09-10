using MTGPlexer.RegexGeneration.RegexTemplateLines;
namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// The base of all regex segment types, including regex patterns not associated with any TokenUnit property,
/// as well as those associated with enum, bool, text placeholder, and child TokenUnit property types. Conceptually,
/// this is a segment of Regex within a broader RegexTemplate which combines with other segments into a finished
/// rendered Regex string & Regex object.
/// </summary>
public abstract class RegexSegmentBase
{
    public string RegexString { get; protected set; }
    public abstract void ComposeRegexLines(RegexLineCollector collector);
    public override string ToString() => RegexString;
}