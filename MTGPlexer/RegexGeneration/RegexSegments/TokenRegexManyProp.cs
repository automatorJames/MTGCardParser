namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also some TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public class TokenRegexManyProp : CaptureGroupPropBase
{
    ManyItemVariant _manyItemType;
    Type _baseType;
    string[] _manyItemNames;
    Dictionary<string, ManyItemOrdinal> _manyItemNamesToOrdinals;
    RegexSegmentBase[] _ordinalRegexProps = new RegexSegmentBase[3];
    static EnumRegexProp _conjunctionProp = (EnumRegexProp)(new RegexPropInfo(typeof(ManyOf).GetProperty(nameof(ManyOf.Conjunction)))).GetCaptureGroupPropBase();

    public override Regex MatchRegex => TokenTypeRegistry.ManyOfRegexes[_baseType];

    public TokenRegexManyProp(RegexPropInfo captureProp) : base(captureProp)
    {
        _baseType = captureProp.BaseType;
        _manyItemNames = Enum.GetValues<ManyItemOrdinal>().Select(x => $"{captureProp.Name}{x.Description()}").ToArray();

        _manyItemNamesToOrdinals = new Dictionary<string, ManyItemOrdinal>
        {
            [_manyItemNames[0]] = ManyItemOrdinal.First,
            [_manyItemNames[1]] = ManyItemOrdinal.SecondPlus,
            [_manyItemNames[2]] = ManyItemOrdinal.Last,
        };

        if (!_baseType.IsAssignableTo(typeof(TokenUnit)) && !_baseType.IsEnum)
            throw new Exception($"TokenRegexManyProp base type may only be derived from TokenUnit or be an enum");

        if (_baseType.IsAssignableTo(typeof(TokenUnit)))
        {
            _manyItemType = ManyItemVariant.TokenUnit;

            _ordinalRegexProps =
            [
                new TokenRegexProp(captureProp.DerviveForManyOfItem(ManyItemOrdinal.First)),
                new TokenRegexProp(captureProp.DerviveForManyOfItem(ManyItemOrdinal.SecondPlus)),
                new TokenRegexProp(captureProp.DerviveForManyOfItem(ManyItemOrdinal.Last)),
            ];
        }
        else if (_baseType.IsEnum)
        {
            _manyItemType = ManyItemVariant.Enum;

            _ordinalRegexProps =
            [
                new EnumRegexProp(captureProp.DerviveForManyOfItem(ManyItemOrdinal.First)),
                new EnumRegexProp(captureProp.DerviveForManyOfItem(ManyItemOrdinal.SecondPlus)),
                new EnumRegexProp(captureProp.DerviveForManyOfItem(ManyItemOrdinal.Last)),
            ];
        }
        else
            throw new Exception($"TokenRegexManyProp base type may only be derived from TokenUnit or be an enum");
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenGroup(RegexPropInfo, spaceDisposition: SpaceDisposition.NeverAddSpaceLocal);
        ConcatenatingComposer.Instance.Compose(builder, [_ordinalRegexProps[0]]);
        builder.OpenGroup(spaceDisposition: SpaceDisposition.NeverAddSpaceLocal);
        builder.AddTextLine(", ");
        ConcatenatingComposer.Instance.Compose(builder, [_ordinalRegexProps[1]]);
        builder.CloseGroup(GroupQuantifier.AnyNumber);
        builder.OpenGroup(spaceDisposition: SpaceDisposition.NeverAddSpaceLocal);
        builder.AddTextLine(",? ");
        builder.OpenGroup(spaceDisposition: SpaceDisposition.NeverAddSpaceLocal);
        _conjunctionProp.ComposeRegexLines(builder);
        builder.AddTextLine(" ");
        builder.CloseGroup(GroupQuantifier.Optional);
        ConcatenatingComposer.Instance.Compose(builder, [_ordinalRegexProps[2]]);
        builder.CloseGroup();
        builder.CloseGroup();
    }

    public override bool SetValueFromMatch(TokenUnit token, Match match)
    {
        Group[] ordinalGroups =
        [
            match.Groups[_manyItemNames[0]],
            match.Groups[_manyItemNames[1]],
            match.Groups[_manyItemNames[2]],
        ];

        // Dynamically create the generic type for List<ManyItemCapture<T>>
        var manyItemCaptureType = typeof(ManyItemCapture<>).MakeGenericType(_baseType);
        var listType = typeof(List<>).MakeGenericType(manyItemCaptureType);
        var hydratedItems = (System.Collections.IList)Activator.CreateInstance(listType);

        for (int i = 0; i < ordinalGroups.Length; i++)
        {
            var ordinal = _manyItemNamesToOrdinals[_manyItemNames[i]];
            var ordinalGroup = ordinalGroups[i];

            foreach (Capture itemCapture in ordinalGroup.Captures)
            {
                object childItem = null;

                if (_manyItemType == ManyItemVariant.TokenUnit)
                {
                    var ancestorCapturePath = token.CapturePath.Dot($"{RegexPropInfo.Name}[{i}]");
                    childItem = token.HydrateAsChildFromCapture(_baseType, match, itemCapture, ancestorCapturePath);
                }
                else if (_manyItemType == ManyItemVariant.Enum)
                {
                    foreach (var enumAlternative in TokenTypeRegistry.EnumScalarAlternativeSets[_baseType].EnumAlternates)
                    {
                        if (enumAlternative.ItemRegex.IsMatch(itemCapture.Value))
                        {
                            childItem = enumAlternative.EnumValue;
                            break;
                        }
                    }

                    if (childItem == null)
                        throw new Exception($"Found no matching values for enum type '{_baseType.Name}' from capture '{itemCapture.Value}'");
                }

                // Create an instance of ManyItemCapture<T> and add it to the list
                var hydratedItem = Activator.CreateInstance(manyItemCaptureType, childItem, itemCapture, ordinal, RegexPropInfo);
                hydratedItems.Add(hydratedItem);
            }
        }

        var conjunctionCapture = match.Groups[nameof(Conjunction)];
        Conjunction? conjunctionValue = Enum.TryParse<Conjunction>(conjunctionCapture.Value, true, out var parsed) ? parsed : null;

        // Dynamically create the generic type for ManyToken<T>
        var manyTokenType = typeof(ManyOf<>).MakeGenericType(_baseType);
        var manyPropVal = Activator.CreateInstance(manyTokenType, hydratedItems, conjunctionValue, conjunctionCapture);

        token.SetPropertyFromCapture(RegexPropInfo, match, manyPropVal);

        return true;
    }

    public override string ToString() => base.ToString();
}