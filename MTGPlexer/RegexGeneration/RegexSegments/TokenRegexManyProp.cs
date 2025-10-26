using System.Text.RegularExpressions;

namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also some TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public record TokenRegexManyProp : CaptureGroupPropBase
{
    static string[] _ordinalNameAppendices = Enum.GetValues<ManyItemOrdinal>().Select(x => x.Description()).ToArray();
    ManyItemVariant _manyItemType;
    Type _baseType;
    string[] _manyItemNames;
    RegexSegmentBase[] _ordinalRegexProps = new RegexSegmentBase[3];
    static EnumRegexProp _conjunctionProp = (EnumRegexProp)(new RegexPropInfo(typeof(ManyOf).GetProperty(nameof(ManyOf.Conjunction)))).GetCaptureGroupPropBase();

    public override Regex ManyMatchRegex => TokenTypeRegistry.ManyOfRegexes[_baseType];

    public TokenRegexManyProp(RegexPropInfo captureProp) : base(captureProp)
    {
        _baseType = captureProp.BaseType;
        _manyItemNames = _ordinalNameAppendices.Select(x => $"{captureProp.Name}{x}").ToArray();

        var derivedPropFirst = captureProp.DerviveForManyOfItem(ManyItemOrdinal.First);
        var derivedPropSecond = captureProp.DerviveForManyOfItem(ManyItemOrdinal.SecondPlus);
        var derivedPropLast = captureProp.DerviveForManyOfItem(ManyItemOrdinal.Last);

        if (_baseType.IsAssignableTo(typeof(TokenUnit)))
        {
            _manyItemType = ManyItemVariant.TokenUnit;

            _ordinalRegexProps =
            [
                new TokenRegexProp(derivedPropFirst),
                new TokenRegexProp(derivedPropSecond),
                new TokenRegexProp(derivedPropLast),
            ];
        }
        else if (_baseType.IsEnum)
        {
            _manyItemType = ManyItemVariant.Enum;

            _ordinalRegexProps =
            [
                new EnumRegexProp(derivedPropFirst),
                new EnumRegexProp(derivedPropSecond),
                new EnumRegexProp(derivedPropLast),
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

    public override bool SetValueFromMatch(TokenUnit token, Match match, string distinguishingAppendix = null)
    {
        Group[] ordinalGroups =
        [
            match.Groups[_manyItemNames[0] + distinguishingAppendix],
            match.Groups[_manyItemNames[1] + distinguishingAppendix],
            match.Groups[_manyItemNames[2] + distinguishingAppendix],
        ];

        var manyItemCaptureType = typeof(ManyItemCapture<>).MakeGenericType(_baseType);
        var listType = typeof(List<>).MakeGenericType(manyItemCaptureType);
        var hydratedItems = (System.Collections.IList)Activator.CreateInstance(listType);

        // For each of the three positions (_first, _secondPlus, _last)
        for (int i = 0; i < ordinalGroups.Length; i++)
        {
            var ordinalNameAppendix = _ordinalNameAppendices[i];
            var ordinal = (ManyItemOrdinal)i;
            var ordinalGroup = ordinalGroups[i];

            // For each of the captures within each of the three positions (_secondPlus may have any number, including none)
            foreach (Capture itemCapture in ordinalGroup.Captures)
            {
                object childItem = null;

                if (_manyItemType == ManyItemVariant.Enum)
                {
                    // Enum logic remains the same as it does not involve nested structures or re-matching.
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
                else if (_manyItemType == ManyItemVariant.TokenUnit)
                {
                    var ancestorCapturePath = token.CapturePath.Dot(RegexPropInfo.Name);
                    var tokenUnitChild = token.HydrateAsChildFromCapture(RegexPropInfo.BaseType, match, itemCapture, ancestorCapturePath, ordinalNameAppendix);

                    // Set the top-level properties for this child item.
                    tokenUnitChild.TopLevelMatch = match;
                    tokenUnitChild.Capture = itemCapture; // This capture has the correct absolute index.
                    tokenUnitChild.CapturePath = token.CapturePath.Dot($"{RegexPropInfo.Name}[{hydratedItems.Count}]");

                    // Get the TokenRegexProp that contains the suffixed definitions for this ordinal.
                    var ordinalTokenProp = (TokenRegexProp)_ordinalRegexProps[i];

                    childItem = tokenUnitChild;
                }

                var hydratedItem = Activator.CreateInstance(manyItemCaptureType, childItem, itemCapture, ordinal, RegexPropInfo);
                hydratedItems.Add(hydratedItem);
            }
        }

        var conjunctionCapture = match.Groups[nameof(Conjunction) + distinguishingAppendix];
        Conjunction? conjunctionValue = Enum.TryParse<Conjunction>(conjunctionCapture.Value, true, out var parsed) ? parsed : null;

        var manyTokenType = typeof(ManyOf<>).MakeGenericType(_baseType);
        var manyPropVal = Activator.CreateInstance(manyTokenType, hydratedItems, conjunctionValue, conjunctionCapture);

        // Use the capture for the entire ManyOf group.
        var manyOfCapture = match.Groups[Name + distinguishingAppendix].Success ? match.Groups[Name] : match;
        token.SetPropertyFromCapture(RegexPropInfo, manyOfCapture, manyPropVal);

        return true;
    }

    public override string ToString() => base.ToString();
}