namespace MTGPlexer.RegexGeneration.RegexSegments;

public record CompoundOfProp : CaptureGroupPropBase
{
    CaptureTypeVariant _compoundItemType;
    CaptureGroupPropBase _regexProp;
    public Type BaseType { get; set; }
    public override Regex ManyMatchRegex => TokenTypeRegistry.ManyOfRegexes[BaseType];


    public CompoundOfProp(TemplatePropInfo captureProp) : base(captureProp)
    {
        // TemplatePropInfo capture prop is a CompoundOf<T> prop here

        BaseType = captureProp.BaseType;
        var derivedPropInfo = captureProp with { TemplatePropType = TemplatePropInfo.GetRegexPropType(TemplatePropInfo.BaseType) };

        if (BaseType.IsAssignableTo(typeof(TokenUnit)))
        {
            _compoundItemType = CaptureTypeVariant.TokenUnit;
            _regexProp = new TokenRegexProp(derivedPropInfo);
        }
        else if (BaseType.IsEnum)
        {
            _compoundItemType = CaptureTypeVariant.Enum;
            _regexProp = new EnumRegexProp(derivedPropInfo);
        }
        else
            throw new Exception($"CompoundProp base type may only be derived from TokenUnit or be an enum");
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenGroup(TemplatePropInfo);
        builder.OpenGroup(spaceDisposition: SpaceDisposition.DisallowedLocal);
        ConcatenatingComposer.Instance.Compose(builder, [_regexProp]);
        builder.AddTextLine(" ?");
        builder.CloseGroup(GroupQuantifier.OneOrMore);
        builder.CloseGroup();
    }

    public override object GetValueToSet(TokenUnit parentTokenUnit, Group namedGroup)
    {
        var polyItemCaptureType = typeof(PolyItemCapture<>).MakeGenericType(BaseType);
        var listType = typeof(List<>).MakeGenericType(polyItemCaptureType);
        var hydratedItems = (System.Collections.IList)Activator.CreateInstance(listType);

        // In compound captures, "namedGroup" is the parent capture (at the compound container level),
        // but the actual item captures reside in the next level down at the prop level.
        var itemContainerCapture = parentTokenUnit.Match[Name + "_" + TemplatePropInfo.Prop.Name];

        var ordinalCaptures = itemContainerCapture.Captures;

        for (int i = 0; i < ordinalCaptures.Count; i++)
        {
            var ordinalCapture = ordinalCaptures[i];
            object childItem = null;

            if (_compoundItemType == CaptureTypeVariant.Enum)
            {
                foreach (var enumAlternative in TokenTypeRegistry.EnumScalarAlternativeSets[BaseType].EnumAlternates)
                {
                    if (enumAlternative.ItemRegex.IsMatch(ordinalCapture.Value))
                    {
                        childItem = enumAlternative.EnumValue;
                        break;
                    }
                }

                if (childItem == null)
                    goto NextIteration;
                    //throw new Exception($"Found no matching values for enum type '{BaseType.Name}' from capture '{ordinalCapture.Value}'");
            }
            else if (_compoundItemType == CaptureTypeVariant.TokenUnit)
            {
                CaptureGroupPropPath capturePath = parentTokenUnit.Match.CapturePath.Append(TemplatePropInfo.Name, _regexProp.Name);
                TokenUnitMatch typeMatch = new(BaseType, parentTokenUnit.Match.RegexMatch, parentTokenUnit.Match.SourceText, capturePath, i);
                var tokenUnitChild = TokenUnit.InstantiateFromMatch(typeMatch);
                childItem = tokenUnitChild;
            }

            var hydratedItem = Activator.CreateInstance(polyItemCaptureType, childItem, ordinalCapture, TemplatePropInfo, i);
            hydratedItems.Add(hydratedItem);

            NextIteration:;
        }

        var compoundType = typeof(CompoundOf<>).MakeGenericType(BaseType);
        var compoundPropVal = Activator.CreateInstance(compoundType, hydratedItems);

        return compoundPropVal;
    }


    public override string ToString() => base.ToString();
}