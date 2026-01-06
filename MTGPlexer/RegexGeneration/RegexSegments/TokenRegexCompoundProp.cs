namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also some TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public record TokenRegexCompoundProp : CaptureGroupPropBase
{
    CaptureTypeVariant _compoundItemType;
    CaptureGroupPropBase _regexProp;
    public Type BaseType { get; set; }
    public override Regex ManyMatchRegex => TokenTypeRegistry.ManyOfRegexes[BaseType];


    public TokenRegexCompoundProp(RegexPropInfo captureProp) : base(captureProp)
    {
        // RegexPropInfo capture prop is a CompoundOf<T> prop here

        BaseType = captureProp.BaseType;
        var derivedPropInfo = captureProp.DerviveForCompoundOfItem();

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
            throw new Exception($"TokenRegexManyProp base type may only be derived from TokenUnit or be an enum");
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenGroup(RegexPropInfo, spaceDisposition: SpaceDisposition.DisallowedLocal);
        ConcatenatingComposer.Instance.Compose(builder, [_regexProp]);
        builder.AddTextLine(" ?");
        builder.CloseGroup(GroupQuantifier.OneOrMore);
    }

    public override bool SetValueFromNamedGroupInMatch(TokenUnit token)
    {
        var manyItemCaptureType = typeof(ManyItemCapture<>).MakeGenericType(BaseType);
        var listType = typeof(List<>).MakeGenericType(manyItemCaptureType);
        var hydratedItems = (System.Collections.IList)Activator.CreateInstance(listType);
        var parentCompoundOfGroup = token.Match.RegexMatch.Groups[Name];
        var ordinalCaptures = token.Match.GetCapturesAtRelativePath(_regexProp).ToList();

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
                    throw new Exception($"Found no matching values for enum type '{BaseType.Name}' from capture '{ordinalCapture.Value}'");
            }
            else if (_compoundItemType == CaptureTypeVariant.TokenUnit)
            {
                CaptureGroupPropPath capturePath = token.Match.CapturePath.Append(RegexPropInfo.Name, _regexProp.Name);
                TokenUnitMatch typeMatch = new(BaseType, token.Match.RegexMatch, token.Match.SourceText, capturePath, i);
                var tokenUnitChild = TokenUnit.InstantiateFromMatch(typeMatch);
                childItem = tokenUnitChild;
            }

            var hydratedItem = Activator.CreateInstance(manyItemCaptureType, childItem, ordinalCapture, i, RegexPropInfo);
            hydratedItems.Add(hydratedItem);
        }

        var conjunctionCapture = token.Match.GetCaptureAtRelativePath(nameof(Conjunction));

        Conjunction? conjunctionValue = conjunctionCapture == null ? null
            : Enum.TryParse<Conjunction>(conjunctionCapture.Value, true, out var parsed) 
            ? parsed : null;

        var manyTokenType = typeof(ManyOf<>).MakeGenericType(BaseType);
        var manyPropVal = Activator.CreateInstance(manyTokenType, hydratedItems, conjunctionValue, conjunctionCapture);
        token.SetPropertyFromCapture(RegexPropInfo, parentCompoundOfGroup, manyPropVal);

        return true;
    }


    public override string ToString() => base.ToString();
}