using System.Collections.Immutable;
namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also a TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public record TokenRegexProp : CaptureGroupPropBase
{
    public override Regex ManyMatchRegex => TokenTypeRegistry.Templates[RegexPropInfo.BaseType].Regex;

    public ImmutableList<RegexSegmentBase> ChildSegments { get; init; }

    public TokenRegexProp(RegexPropInfo captureProp) : base(captureProp)
    {
        var template = TokenTypeRegistry.GetTypeTemplate(captureProp.BaseType);
        ChildSegments = template.RegexSegments.Select(x => ApplyDistinguishingAppendix(captureProp, x)).ToImmutableList();
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenGroup(RegexPropInfo);
        ConcatenatingComposer.Instance.Compose(builder, ChildSegments.ToList());
        builder.CloseGroup();
    }

    public override bool SetValueFromMatch(TokenUnit token, Match match, string distinguishingAppendix = null)
    {
        var capture = match.Groups[Name + distinguishingAppendix];

        if (capture == null)
            return false;

        var ancestorCapturePath = token.CapturePath.Dot(RegexPropInfo.Name);
        var tokenUnitInstance = token.HydrateAsChildFromCapture(RegexPropInfo.BaseType, match, capture, ancestorCapturePath, distinguishingAppendix);
        token.SetPropertyFromCapture(RegexPropInfo, capture, tokenUnitInstance);

        return true;
    }

    RegexSegmentBase ApplyDistinguishingAppendix(RegexPropInfo prop, RegexSegmentBase segment)
    {
        // Case 1: If there's no appendix to apply, we're done.
        if (string.IsNullOrEmpty(prop.ManyOfItemDistinguisher))
            return segment;

        // Case 2: If the segment is not a CaptureGroupProp (e.g., a TextSegment),
        // it can't have a distinguishing name, so we're done with this branch.
        if (segment is not CaptureGroupPropBase captureGroupProp)
            return segment;

        // --- Processing Step: Modify the current node ---
        // This happens for ALL CaptureGroupPropBase types.
        var distinguishedRegexPropInfo = captureGroupProp.RegexPropInfo with
        {
            Name = captureGroupProp.RegexPropInfo.Name + prop.ManyOfItemDistinguisher,
            // Propagate the appendix for any potential grandchildren.
            ManyOfItemDistinguisher = prop.ManyOfItemDistinguisher
        };

        // --- Recursive Step: Check for containers and process their children ---
        switch (captureGroupProp)
        {
            // This case handles both TokenRegexProp and its inheritor, TokenRegexOneOfProp.
            case TokenRegexProp tokenProp:
                {
                    // 1. Recurse: Call this same function on every child segment.
                    var newChildSegments = tokenProp.ChildSegments
                        .Select(child => ApplyDistinguishingAppendix(distinguishedRegexPropInfo, child))
                        .ToImmutableList();

                    // 2. Reconstruct: Return a new TokenRegexProp containing the modified
                    //    RegexPropInfo AND the new, recursively-modified child segments.
                    return tokenProp with
                    {
                        RegexPropInfo = distinguishedRegexPropInfo,
                        ChildSegments = newChildSegments
                    };
                }

            //todo: implement this so we can handle nested ManyOfs
            //case TokenRegexManyProp manyProp:
            //    {
            //        // This follows the same pattern as TokenRegexProp but for its specific children.
            //        // Note: This assumes _ordinalRegexProps is exposed as a public property in the record.
            //        var newOrdinalRegexProps = manyProp.OrdinalRegexProps
            //            .Select(child => ApplyDistinguishingAppendix(distinguishedRegexPropInfo, child))
            //            .ToImmutableList(); // Or .ToArray() depending on the property type
            //
            //        return manyProp with
            //        {
            //            RegexPropInfo = distinguishedRegexPropInfo,
            //            OrdinalRegexProps = newOrdinalRegexProps
            //        };
            //    }

            // Base Case 3: The segment is a CaptureGroupProp (like EnumRegexProp), but not a container.
            // We just need to return a new version of it with its modified RegexPropInfo.
            default:
                {
                    return captureGroupProp with { RegexPropInfo = distinguishedRegexPropInfo };
                }
        }
    }

    public override string ToString() => base.ToString();
}