namespace MTGPlexer.RegexGeneration.RegexSegments;

public record ManyOfSegment : XOfSegmentBase, IMultiCaptureSegment
{
    CaptureTypeVariant _manyItemType;
    CaptureGroupSegmentBase[] _ordinalRegexProps = new CaptureGroupSegmentBase[3];
    static EnumSegment _conjunctionProp = (EnumSegment)(new TemplatePropInfo(typeof(ManyOf).GetProperty(nameof(ManyOf.Conjunction)))).GetCaptureGroupPropBase();

    public override Regex ManyMatchRegex => TokenTypeRegistry.ManyOfRegexes[GenericType];

    public ManyOfSegment(TemplatePropInfo captureProp) : base(captureProp)
    {
        var derivedPropFirst = captureProp.DeriveForXOfItem(ManyItemOrdinal.First.ToString());
        var derivedPropSecond = captureProp.DeriveForXOfItem(ManyItemOrdinal.SecondPlus.ToString());
        var derivedPropLast = captureProp.DeriveForXOfItem(ManyItemOrdinal.Last.ToString());

        if (GenericType.IsAssignableTo(typeof(TokenUnit)))
        {
            _manyItemType = CaptureTypeVariant.TokenUnit;

            _ordinalRegexProps =
            [
                new TokenUnitSegment(derivedPropFirst),
                new TokenUnitSegment(derivedPropSecond),
                new TokenUnitSegment(derivedPropLast),
            ];
        }
        else if (GenericType.IsEnum)
        {
            _manyItemType = CaptureTypeVariant.Enum;

            _ordinalRegexProps =
            [
                new EnumSegment(derivedPropFirst),
                new EnumSegment(derivedPropSecond),
                new EnumSegment(derivedPropLast),
            ];
        }
        else
            throw new Exception($"ManyOfProp base type may only be derived from TokenUnit or be an enum");
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenGroup(TemplatePropInfo, spaceDisposition: SpaceDisposition.DisallowedLocal);
        ConcatenatingComposer.Instance.Compose(builder, [_ordinalRegexProps[0]]);
        builder.OpenGroup(spaceDisposition: SpaceDisposition.DisallowedLocal);
        builder.AddTextLine(", ");
        ConcatenatingComposer.Instance.Compose(builder, [_ordinalRegexProps[1]]);
        builder.CloseGroup(GroupQuantifier.AnyNumber);
        builder.OpenGroup(spaceDisposition: SpaceDisposition.DisallowedLocal);
        builder.AddTextLine(",? ");
        builder.OpenGroup(spaceDisposition: SpaceDisposition.DisallowedLocal);
        _conjunctionProp.ComposeRegexLines(builder);
        builder.AddTextLine(" ");
        builder.CloseGroup(GroupQuantifier.Optional);
        ConcatenatingComposer.Instance.Compose(builder, [_ordinalRegexProps[2]]);
        builder.CloseGroup();
        builder.CloseGroup();
    }

    public override void SetPropertyFromCaptures(TokenUnit parentTokenUnit, Capture[] scopedCaptures)
    {
        List<PolyItemCapture> hydratedItems = [];

        for (int i = 0; i < _ordinalRegexProps.Length; i++)
        {
            var ordinalProp = _ordinalRegexProps[i];
            var manyItemOrdinal = (ManyItemOrdinal)i;

            // In manyof captures, "namedGroup" is the parent capture (at the many-of container level),
            // but the actual item captures reside in the next level down at the ordinal level.
            var ordinalGroup = parentTokenUnit.Match[Name + "_" + manyItemOrdinal.ToString()];

            // a null ordinal group should only possibly occur for the second ordinal
            if (ordinalGroup == null)
                continue;

            var ordinalCaptures = ordinalGroup.Captures;

            // "first" will always have 1 item
            // "second" will have any number of items (including 0)
            // "last" will always have 1 item
            for (int j = 0; j < ordinalCaptures.Count; j++)
            {
                var ordinalCapture = ordinalCaptures[j];
                object childItem = null;

                if (_manyItemType == CaptureTypeVariant.Enum)
                {
                    foreach (var enumAlternative in TokenTypeRegistry.EnumScalarAlternativeSets[GenericType].EnumAlternates)
                    {
                        if (enumAlternative.ItemRegex.IsMatch(ordinalCapture.Value))
                        {
                            childItem = enumAlternative.EnumValue;
                            break;
                        }
                    }

                    if (childItem == null)
                        throw new Exception($"Found no matching values for enum type '{GenericType.Name}' from capture '{ordinalCapture.Value}'");
                }
                else if (_manyItemType == CaptureTypeVariant.TokenUnit)
                {
                    var nameAppendix = TemplatePropInfo.Name.Dot(ordinalProp.Name);
                    TokenUnitMatch typeMatch = new(GenericType, parentTokenUnit, nameAppendix, j);
                    var tokenUnitChild = TokenUnit.InstantiateFromMatch(typeMatch);
                    childItem = tokenUnitChild;
                }

                PolyItemCapture hydratedItem = new(childItem, ordinalCapture, TemplatePropInfo, j, manyItemOrdinal.ToString());
                hydratedItems.Add(hydratedItem);
            }
        }

        var conjunctionCapture = parentTokenUnit.Match[Name + "_" + nameof(Conjunction)];

        Conjunction? conjunctionValue = conjunctionCapture == null ? null
            : Enum.TryParse<Conjunction>(conjunctionCapture.Value, true, out var parsed) 
            ? parsed : null;

        var manyTokenType = typeof(ManyOf<>).MakeGenericType(TemplatePropInfo.GenericTypes);
        var manyPropVal = Activator.CreateInstance(manyTokenType, hydratedItems, conjunctionValue, conjunctionCapture);

        return manyPropVal;
    }


    public override string ToString() => base.ToString();
}