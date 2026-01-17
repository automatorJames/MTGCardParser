namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a bool property on a TokenUnit or a bool x-Of PolyItemCapure. Bool property Regexes typically check for the optional presence
/// of some matching pattern. Such properties are usually expected to have a RegexPattern attribute that defines
/// its pattern(s), but in the absence of this the normalized property name is matched.
/// </summary>
public record BoolSegment : ScalarCaptureSegmentBase
{
    public BoolSegment(TemplatePropInfo captureProp) : base(captureProp)
    {
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenGroup(TemplatePropInfo, isOptional: true);
        builder.AddAlternateValues(ScalarAlternativeSet.Alternates);
        builder.CloseGroup(GroupQuantifier.Optional);
    }

    public override object GetPropertyValue(MatchTraversalState parentTokenUnitMatch, ExtractedCapture scopedCapture)
    {
        // This override simply returns "true", because CaptureGroupPropBase already validated
        // that the named group exists, therefore this bool check succeeds

        return true;
    }
}