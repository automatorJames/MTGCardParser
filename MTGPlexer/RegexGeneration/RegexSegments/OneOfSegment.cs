namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also some TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public record OneOfSegment : CaptureGroupSegmentBase
{
    List<CaptureGroupSegmentBase> _regexProps = [];
    public override Regex ManyMatchRegex => throw new NotImplementedException();
    public HashSet<Type> EnumTypesRepresented = [];

    public OneOfSegment(TemplatePropInfo captureProp) : base(captureProp)
    {
        // TemplatePropInfo capture prop is a OneOf<> prop here

        foreach (var type in captureProp.BaseType.GetGenericArguments())
        {
            var derivedPropInfo = captureProp with { TemplatePropType = TemplatePropInfo.GetRegexPropType(type), BaseType = type, Name = type.Name };

            if (type.IsAssignableTo(typeof(TokenUnit)))
                _regexProps.Add(new TokenUnitSegment(derivedPropInfo));
            else if (type.IsEnum)
            {
                EnumTypesRepresented.Add(type);
                _regexProps.Add(new EnumSegment(derivedPropInfo));
            }
        }
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenGroup(TemplatePropInfo, spaceDisposition: SpaceDisposition.DisallowedLocal);
        AlternatingComposer.Instance.Compose(builder, _regexProps.Cast<RegexSegmentBase>().ToList());
        builder.CloseGroup();
    }

    public override object GetValueToSet(TokenUnit parentTokenUnit, Group namedGroup)
    {
        object foundPolyMatchValue = null;

        foreach (var regexProp in _regexProps)
        {
            // In compound captures, "namedGroup" is the parent capture (at the compound container level),
            // but the actual item captures reside in the next level down at the prop level.
            var oneOfItemVariantCapture = parentTokenUnit.Match[Name + "_" + regexProp.Name];

            if (oneOfItemVariantCapture == null)
                continue;

            var polyItemCaptureType = typeof(PolyItemCapture<>).MakeGenericType(regexProp.TemplatePropInfo.BaseType);

            if (regexProp.TemplatePropInfo.TemplatePropType == TemplatePropType.Enum)
            {
                foreach (var enumAlternative in TokenTypeRegistry.EnumScalarAlternativeSets[regexProp.TemplatePropInfo.BaseType].EnumAlternates)
                {
                    if (enumAlternative.ItemRegex.IsMatch(oneOfItemVariantCapture.Value))
                    {
                        foundPolyMatchValue = Activator.CreateInstance(polyItemCaptureType, enumAlternative.EnumValue, oneOfItemVariantCapture, regexProp.TemplatePropInfo);
                        goto ItemHasBeenFound;
                    }
                }
            }
            else if (regexProp.TemplatePropInfo.Prop.PropertyType.IsAssignableTo(typeof(TokenUnit)))
            {
                CaptureGroupPropPath capturePath = parentTokenUnit.Match.CapturePath.Append(TemplatePropInfo.Name, regexProp.Name);
                TokenUnitMatch typeMatch = new(regexProp.TemplatePropInfo.BaseType, parentTokenUnit.Match.RegexMatch, parentTokenUnit.Match.SourceText, capturePath);
                var tokenUnitChild = TokenUnit.InstantiateFromMatch(typeMatch);
                foundPolyMatchValue = Activator.CreateInstance(polyItemCaptureType, tokenUnitChild, oneOfItemVariantCapture, regexProp.TemplatePropInfo);
                goto ItemHasBeenFound;
            }
        }

        // If none of the regex props found a match, there's a problem
        throw new Exception($"Failed to extract value for OneOfProp from match '{namedGroup.Value}'");

        ItemHasBeenFound:;

        var genericTypeDefinition = _regexProps.Count switch
        {
            2 => typeof(OneOf<,>),
            3 => typeof(OneOf<,,>),
            _ => throw new Exception($"One-of regex prop count of {_regexProps.Count} not supported")
        };

        var oneOfCaptureType = genericTypeDefinition.MakeGenericType(_regexProps.Select(x => x.TemplatePropInfo.BaseType).ToArray());
        var oneOfPropVal = Activator.CreateInstance(oneOfCaptureType, foundPolyMatchValue);
        
        return oneOfPropVal;
    }


    public override string ToString() => base.ToString();
}