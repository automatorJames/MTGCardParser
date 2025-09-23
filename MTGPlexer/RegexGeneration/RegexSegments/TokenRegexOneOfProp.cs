using MTGPlexer.RegexGeneration.Composers;

namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also some TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public class TokenRegexOneOfProp : TokenRegexProp
{
    public TokenRegexOneOfProp(RegexPropInfo captureProp) : base(captureProp)
    {
        var template = TokenTypeRegistry.GetTypeTemplate(captureProp.UnderlyingType);
        ChildSegments = template.RegexSegments;
    }

    public override void ComposeRegexLines(RegexLineCollector collector)
    {
        // If there are no text sgements to render, the OneOf container itself doesn't need spaces between its alternating members
        SpaceDisposition? spaceDisposition = !ChildSegments.Any(x => x is TextSegment) ? SpaceDisposition.NeverAddSpaceLocal : null;

        collector.OpenGroup(RegexPropInfo, spaceDisposition: spaceDisposition);
        AlternatingComposer.Instance.Compose(collector, ChildSegments);
        collector.CloseGroup();
    }

    public override string ToString() => base.ToString();
}