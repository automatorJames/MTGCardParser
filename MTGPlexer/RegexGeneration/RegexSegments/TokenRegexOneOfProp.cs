namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also some TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public record TokenRegexOneOfProp : TokenRegexProp
{
    public TokenRegexOneOfProp(RegexPropInfo captureProp) : base(captureProp)
    {
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        // If there are no text sgements to render, the OneOf container itself doesn't need spaces between its alternating members
        SpaceDisposition? spaceDisposition = !ChildSegments.Any(x => x is TextSegment) ? SpaceDisposition.DisallowedLocal : null;

        builder.OpenGroup(RegexPropInfo, spaceDisposition: spaceDisposition);
        AlternatingComposer.Instance.Compose(builder, ChildSegments.ToList());
        builder.CloseGroup();
    }

    public override bool SetValueFromNamedGroupInMatch(TokenUnit token)
    {
        var capture = token.Match.GetCaptureAtRelativePath(this);

        if (capture == null)
            return false;

        CaptureGroupPropPath ancestorCapturePath = new(token.Match.CapturePath.PropPath.Dot(RegexPropInfo.Name));
        TokenUnitMatch typeMatch = new(RegexPropInfo.BaseType, token.Match.RegexMatch, token.Match.SourceText, ancestorCapturePath, token.Match.CaptureIndex);
        var tokenUnitInstance = TokenUnit.InstantiateFromMatch(typeMatch);
        token.ChildTokenUnits.Add(tokenUnitInstance);
        token.SetPropertyFromCapture(RegexPropInfo, capture, tokenUnitInstance);

        return true;
    }

    public override string ToString() => base.ToString();
}