namespace MTGPlexer.RegexSegmentDTOs;

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

    public override bool SetValueFromMatchSpan(TokenUnit parentToken, TextSpan matchSpan)
    {
        var subMatchSpan = GetPropSubMatch(matchSpan);

        if (subMatchSpan is null)
            throw new Exception($"No alternative for {nameof(TokenUnitOneOf)} type '{RegexPropInfo.UnderlyingType.Name}' matched '{matchSpan.ToStringValue()}'");

        var oneOfPropInstance = TokenUnit.InstantiateFromMatchString(RegexPropInfo.UnderlyingType, subMatchSpan.Value, parentToken, RegexPropInfo);
        parentToken.SetPropertyCapture(RegexPropInfo, subMatchSpan.Value, oneOfPropInstance);
        return true;
    }

    public override void ComposeRegexLines(RegexLineCollector collector)
    {
        collector.OpenGroup(RegexPropInfo);

        // If there are no text segments, the named group parentheses are a sufficient wrapper to isolate
        // the alterantive properties. If not, we must render the alternate properties within supplemental
        // parentheses to isolate them from the text segments on either side.
        bool shouldWrapAlternatives = ChildSegments.Any(x => x is TextSegment);


        // Flag to track whether we're currently within the set of contiguous alternative properties
        // (only relevant if shouldWrapAlternatives is true)
        bool haveBegunAlternations = false;

        foreach (var segment in ChildSegments)
        {
            if (segment is TextSegment)
            {
                if (haveBegunAlternations)
                    // Close the alternations group before the trailing text segments
                    collector.CloseGroup();

                segment.ComposeRegexLines(collector);

            }
            else if (segment is CaptureGroupPropBase captureProp)
            {
                if (!haveBegunAlternations && shouldWrapAlternatives)
                {
                    collector.OpenGroup();
                    haveBegunAlternations = true;
                }

                segment.ComposeRegexLines(collector);
            }
        }

        if (haveBegunAlternations)
            // Close the alternations group because we're done
            collector.CloseGroup();
    }

    public override string ToString() => base.ToString();
}
