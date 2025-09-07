using MTGPlexer.RegexSegmentDTOs.RegexTemplateLines;

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

    protected override void SetRegex(RegexPropInfo captureProp)
    {
        RegexString = TokenTypeRegistry.GetTypeTemplate(captureProp.UnderlyingType).RegexString;
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

    public override void ComposeRegexLines(List<RegexTemplateLine> lines = null, List<string> namePath = null, int indentation = 0)
    {
        namePath ??= [];
        namePath.Add(RegexPropInfo.Name);
        lines ??= [];
        lines.Add(new NamedGroupOpen(RegexPropInfo.Name, string.Join('.', namePath), indentation));

        // If there are no text segments, the named group parentheses are a sufficient wrapper to isolate
        // the alterantive properties. If not, we must render the alternate properties within supplemental
        // parentheses to isolate them from the text segments on either side.
        bool shouldWrapAlternatives = ChildSegments.Any(x => x is TextSegment);

        // bool props only relevant if shouldWrapAlternatives is true

        // Flag to track whether we're currently within the set of contiguous alternative properties
        bool haveBegunAlternations = false;

        // Flag to track whether we've completed the contiguous alternative properties
        bool haveFinishedAlternations = false;


        foreach (var segment in ChildSegments)
        {
            if (segment is TextSegment)
            {
                if (haveBegunAlternations)
                {
                    // Close the alternations group before the trailing text segments
                    indentation--;
                    lines.Add(new GroupClose(string.Join('.', namePath), indentation));
                    haveFinishedAlternations = true;
                }

                segment.ComposeRegexLines(lines, namePath, indentation);

            }
            else if (segment is CaptureGroupPropBase)
            {
                if (!haveBegunAlternations && shouldWrapAlternatives)
                {
                    indentation++;
                    lines.Add(new GroupOpen(string.Join('.', namePath), indentation));
                    haveBegunAlternations = true;
                }

                segment.ComposeRegexLines(lines, namePath, indentation);

            }
        }

        if (haveBegunAlternations)
        {
            // Close the alternations group because we're done
            indentation--;
            lines.Add(new GroupClose(string.Join('.', namePath), indentation));
            haveFinishedAlternations = true;
        }   
    }

    public override string ToString() => base.ToString();
}
