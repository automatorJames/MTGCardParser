namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also some TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public record OneOfSegment : XOfSegmentBase
{
    List<CaptureGroupSegmentBase> _regexProps = [];
    public override Regex ManyMatchRegex => throw new NotImplementedException();
    public HashSet<Type> EnumTypesRepresented = [];

    public OneOfSegment(TemplatePropInfo captureProp) : base(captureProp)
    {
        for (int i = 0; i < GenericTypes.Length; i++)
        {
            var genericType = GenericTypes[i];
            var derivedPropInfo = captureProp.DeriveForXOfItem(genericTypeIndex: i);

            if (genericType.IsAssignableTo(typeof(TokenUnit)))
                _regexProps.Add(new TokenUnitSegment(derivedPropInfo));
            else if (genericType.IsEnum)
            {
                EnumTypesRepresented.Add(genericType);
                _regexProps.Add(new EnumSegment(derivedPropInfo));
            }
        }
    }

    protected override void SetGenericType(TemplatePropInfo captureProp)
    {
        // Overriding with a no-op since the base calls captureProp.GenericTypes.Single(),
        // which would throw an exception in the case of OneOfSegment.
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenGroup(TemplatePropInfo, spaceDisposition: SpaceDisposition.DisallowedLocal);
        AlternatingComposer.Instance.Compose(builder, _regexProps.Cast<RegexSegmentBase>().ToList());
        builder.CloseGroup();
    }

    public override object GetPropertyValue(MatchTraversalState parentTokenUnitMatch, ExtractedCapture scopedCapture)
    {
        PolyItemCapture foundPolyMatchValue = null;
        int foundPropIndex = 0;

        for (int i = 0; i < _regexProps.Count; i++)
        {
            var regexProp = _regexProps[i];
            var oneOfItemVariantCapture = parentTokenUnitMatch[LeafName + "_" + regexProp.LeafName].SingleOrDefault();

            if (oneOfItemVariantCapture == null)
                continue;

            MatchTraversalState state = new(GenericType, parentTokenUnitMatch, regexProp.LeafName, i);
            var childItem = regexProp.GetPropertyValue(state, oneOfItemVariantCapture);
            foundPolyMatchValue = new(childItem, oneOfItemVariantCapture, TemplatePropInfo);

            goto ItemHasBeenFound;
        }

        // If none of the regex props found a match, there's a problem
        throw new Exception($"Failed to extract value for OneOfProp from capture '{scopedCapture.Value}'");

        ItemHasBeenFound:;

        var genericTypeDefinition = _regexProps.Count switch
        {
            2 => typeof(OneOf<,>),
            3 => typeof(OneOf<,,>),
            _ => throw new Exception($"One-of regex prop count of {_regexProps.Count} not supported")
        };

        var oneOfCaptureType = genericTypeDefinition.MakeGenericType(_regexProps.Select(x => x.TemplatePropInfo.UnderlyingType).ToArray());
        var oneOfPropVal = Activator.CreateInstance(oneOfCaptureType, foundPolyMatchValue, foundPropIndex);
        
        return oneOfPropVal;
    }


    public override string ToString() => base.ToString();
}