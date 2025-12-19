using System.ComponentModel;

namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also some TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public record TokenRegexManyProp : CaptureGroupPropBase
{
    public Type BaseType { get; set; }

    static string[] _ordinalNameAppendices = Enum.GetValues<ManyItemOrdinal>().Select(x => x.Description()).ToArray();
    ManyItemVariant _manyItemType;
    string[] _manyItemNames;
    CaptureGroupPropBase[] _ordinalRegexProps = new CaptureGroupPropBase[3];
    static EnumRegexProp _conjunctionProp = (EnumRegexProp)(new RegexPropInfo(typeof(ManyOf).GetProperty(nameof(ManyOf.Conjunction)))).GetCaptureGroupPropBase();

    public override Regex ManyMatchRegex => TokenTypeRegistry.ManyOfRegexes[BaseType];

    public TokenRegexManyProp(RegexPropInfo captureProp) : base(captureProp)
    {
        // RegexPropInfo capture prop is a ManyOf<T> prop here

        BaseType = captureProp.BaseType;
        _manyItemNames = _ordinalNameAppendices.Select(x => $"{captureProp.Name}{x}").ToArray();

        var derivedPropFirst = captureProp.DerviveForManyOfItem(ManyItemOrdinal.First);
        var derivedPropSecond = captureProp.DerviveForManyOfItem(ManyItemOrdinal.SecondPlus);
        var derivedPropLast = captureProp.DerviveForManyOfItem(ManyItemOrdinal.Last);

        if (BaseType.IsAssignableTo(typeof(TokenUnitOneOf)))
        {
            _manyItemType = ManyItemVariant.TokenUnit;

            _ordinalRegexProps =
            [
                new TokenRegexOneOfProp(derivedPropFirst),
                new TokenRegexOneOfProp(derivedPropSecond),
                new TokenRegexOneOfProp(derivedPropLast),
            ];
        }
        else if (BaseType.IsAssignableTo(typeof(TokenUnit)))
        {
            _manyItemType = ManyItemVariant.TokenUnit;

            _ordinalRegexProps =
            [
                new TokenRegexProp(derivedPropFirst),
                new TokenRegexProp(derivedPropSecond),
                new TokenRegexProp(derivedPropLast),
            ];
        }
        else if (BaseType.IsEnum)
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

    public override bool SetValueFromNamedGroupInMatch(TokenUnit token)
    {
        var manyItemCaptureType = typeof(ManyItemCapture<>).MakeGenericType(BaseType);
        var listType = typeof(List<>).MakeGenericType(manyItemCaptureType);
        var hydratedItems = (System.Collections.IList)Activator.CreateInstance(listType);

        var parentManyOfGroup = token.Match.RegexMatch.Groups[Name];

        for (int i = 0; i < _ordinalRegexProps.Length; i++)
        {
            var ordinalProp = _ordinalRegexProps[i];
            var manyItemOrdinal = (ManyItemOrdinal)i;
            var ordinalCaptures = token.Match.GetCapturesAtRelativePath(ordinalProp).ToList();

            for (int j = 0; j < ordinalCaptures.Count; j++)
            {
                var ordinalCapture = ordinalCaptures[j];
                object childItem = null;

                if (_manyItemType == ManyItemVariant.Enum)
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
                else if (_manyItemType == ManyItemVariant.TokenUnit)
                {
                    CaptureGroupPropPath capturePath = token.Match.CapturePath.Append(RegexPropInfo.Name, ordinalProp.Name);
                    TokenUnitMatch typeMatch = new(BaseType, token.Match.RegexMatch, token.Match.SourceText, capturePath, j);
                    var tokenUnitChild = TokenUnit.InstantiateFromMatch(typeMatch);
                    childItem = tokenUnitChild;
                }

                var hydratedItem = Activator.CreateInstance(manyItemCaptureType, childItem, ordinalCapture, j, manyItemOrdinal, RegexPropInfo);
                hydratedItems.Add(hydratedItem);
            }
        }

        var conjunctionCapture = token.Match.GetCaptureAtRelativePath(nameof(Conjunction));

        Conjunction? conjunctionValue = 
            Enum.TryParse<Conjunction>(conjunctionCapture.Value, true, out var parsed) 
            ? parsed : null;

        var manyTokenType = typeof(ManyOf<>).MakeGenericType(BaseType);
        var manyPropVal = Activator.CreateInstance(manyTokenType, hydratedItems, conjunctionValue, conjunctionCapture);
        token.SetPropertyFromCapture(RegexPropInfo, parentManyOfGroup, manyPropVal);

        return true;
    }


    public override string ToString() => base.ToString();
}