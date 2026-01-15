namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also some TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public record TokenUnitOneOfSegment : TokenUnitSegment
{
    public TokenUnitOneOfSegment(TemplatePropInfo captureProp) : base(captureProp)
    {
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        // If there are no text sgements to render, the OneOf container itself doesn't need spaces between its alternating members
        SpaceDisposition? spaceDisposition = !ChildSegments.Any(x => x is TextSegment) ? SpaceDisposition.DisallowedLocal : null;

        builder.OpenGroup(TemplatePropInfo, spaceDisposition: spaceDisposition);
        AlternatingComposer.Instance.Compose(builder, ChildSegments.ToList());
        builder.CloseGroup();
    }

    public override object GetValueToSet(TokenUnit parentTokenUnit, Group namedGroup)
    {
        CaptureGroupPropPath ancestorCapturePath = new(parentTokenUnit.Match.CapturePath.PropPath.Dot(TemplatePropInfo.Name));
        TokenUnitMatch typeMatch = new(TemplatePropInfo.UnderlyingType, parentTokenUnit.Match.RegexMatch, parentTokenUnit.Match.SourceText, ancestorCapturePath);
        var tokenUnitInstance = TokenUnit.InstantiateFromMatch(typeMatch);
        parentTokenUnit.ChildTokenUnits.Add(tokenUnitInstance);

        return tokenUnitInstance;
    }

    public override string ToString() => base.ToString();
}