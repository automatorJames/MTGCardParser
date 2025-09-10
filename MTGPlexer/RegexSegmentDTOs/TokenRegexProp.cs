using MTGPlexer.RegexSegmentDTOs.Composers;

namespace MTGPlexer.RegexSegmentDTOs;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also a TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public class TokenRegexProp : CaptureGroupPropBase
{
    static HashSet<char> _terminalPunctuation = ['.', ',', ';'];
    bool _noSpaces;

    public List<RegexSegmentBase> ChildSegments { get; set; }

    public TokenRegexProp(RegexPropInfo captureProp) : base(captureProp)
    {
        var template = TokenTypeRegistry.GetTypeTemplate(captureProp.UnderlyingType);
        ChildSegments = template.RegexSegments;
    }

    public override void ComposeRegexLines(RegexLineCollector collector)
    {
        collector.OpenGroup(RegexPropInfo);
        ConcatenatingComposer.Instance.Compose(collector, ChildSegments);
        collector.CloseGroup();
    }

    public override string ToString() => base.ToString();
}