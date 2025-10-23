namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also some TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public record TokenRegexManyProp : CaptureGroupPropBase
{
    ManyItemVariant _manyItemType;
    Type _baseType;
    string[] _manyItemNames;
    RegexSegmentBase[] _ordinalRegexProps = new RegexSegmentBase[3];
    static EnumRegexProp _conjunctionProp = (EnumRegexProp)(new RegexPropInfo(typeof(ManyOf).GetProperty(nameof(ManyOf.Conjunction)))).GetCaptureGroupPropBase();

    public override Regex ManyMatchRegex => TokenTypeRegistry.ManyOfRegexes[_baseType];
    Regex _itemMatchRegex => TokenTypeRegistry.TypeRegexes[_baseType];

    public TokenRegexManyProp(RegexPropInfo captureProp) : base(captureProp)
    {
        _baseType = captureProp.BaseType;
        _manyItemNames = Enum.GetValues<ManyItemOrdinal>().Select(x => $"{captureProp.Name}{x.Description()}").ToArray();

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

    /// <summary>
    /// Recursively hydrates a token's properties using a tree of suffixed property definitions (`TokenRegexProp`).
    /// This method avoids re-matching on substrings by using the main `Match` object and looking up
    /// capture groups by their suffixed names (e.g., "PermanentVerb_first").
    /// </summary>
    private void HydrateTokenFromSuffixedProps(TokenUnit tokenToHydrate, Match match, TokenRegexProp suffixedParentProp)
    {
        // Iterate through the children of the current token definition.
        // Each 'suffixedChildProp' has a name that has been suffixed (e.g., "_first") during regex generation.
        foreach (var suffixedChildProp in suffixedParentProp.ChildSegments.OfType<CaptureGroupPropBase>())
        {
            // Check if this suffixed group was successful in the main match.
            if (!match.Groups[suffixedChildProp.Name].Success) continue;

            // If the property is another TokenUnit, we need to instantiate it and recurse.
            if (suffixedChildProp is TokenRegexProp nestedTokenProp)
            {
                var nestedCapture = match.Groups[suffixedChildProp.Name];

                // Create the child TokenUnit instance.
                var nestedToken = (TokenUnit)Activator.CreateInstance(nestedTokenProp.RegexPropInfo.BaseType);
                nestedToken.TopLevelMatch = match;
                nestedToken.Capture = nestedCapture; // Use the capture from the main match, which has the correct absolute index.
                nestedToken.CapturePath = tokenToHydrate.CapturePath.Dot(nestedTokenProp.RegexPropInfo.Prop.Name); // Use original, unsuffixed prop name for path.
                tokenToHydrate.ChildTokenUnits.Add(nestedToken);

                // Set the property on the parent. This also creates the parent's IndexedPropertyCapture for the nested token.
                tokenToHydrate.SetPropertyFromCapture(nestedTokenProp.RegexPropInfo, nestedCapture, nestedToken);

                // Recurse to hydrate the children of this new nested token.
                HydrateTokenFromSuffixedProps(nestedToken, match, nestedTokenProp);
            }
            else
            {
                // If the property is a terminal (Enum, Bool, Placeholder), its SetValueFromMatch is safe to call directly.
                // It will look up the suffixed group name in the main match and create an IndexedPropertyCapture
                // using the capture object that has the correct absolute index.
                suffixedChildProp.SetValueFromMatch(tokenToHydrate, match);
            }
        }
    }

    public override bool SetValueFromMatch(TokenUnit token, Match match)
    {
        Group[] ordinalGroups =
        [
            match.Groups[_manyItemNames[0]],
            match.Groups[_manyItemNames[1]],
            match.Groups[_manyItemNames[2]],
        ];

        var manyItemCaptureType = typeof(ManyItemCapture<>).MakeGenericType(_baseType);
        var listType = typeof(List<>).MakeGenericType(manyItemCaptureType);
        var hydratedItems = (System.Collections.IList)Activator.CreateInstance(listType);

        for (int i = 0; i < ordinalGroups.Length; i++)
        {
            var ordinal = (ManyItemOrdinal)i;
            var ordinalGroup = ordinalGroups[i];

            foreach (Capture itemCapture in ordinalGroup.Captures)
            {
                object childItem = null;

                if (_manyItemType == ManyItemVariant.TokenUnit)
                {
                    var tokenUnitChild = (TokenUnit)Activator.CreateInstance(_baseType);

                    // Set the top-level properties for this child item.
                    tokenUnitChild.TopLevelMatch = match;
                    tokenUnitChild.Capture = itemCapture; // This capture has the correct absolute index.
                    tokenUnitChild.CapturePath = token.CapturePath.Dot($"{RegexPropInfo.Name}[{hydratedItems.Count}]");
                    token.ChildTokenUnits.Add(tokenUnitChild);

                    // Get the 'TokenRegexProp' that contains the suffixed definitions for this ordinal.
                    var ordinalTokenProp = _ordinalRegexProps[i] as TokenRegexProp;

                    // Call our recursive helper to correctly hydrate the child and all its descendants.
                    HydrateTokenFromSuffixedProps(tokenUnitChild, match, ordinalTokenProp);

                    childItem = tokenUnitChild;
                }
                else if (_manyItemType == ManyItemVariant.Enum)
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

                var hydratedItem = Activator.CreateInstance(manyItemCaptureType, childItem, itemCapture, ordinal, RegexPropInfo);
                hydratedItems.Add(hydratedItem);
            }
        }

        var conjunctionCapture = match.Groups[nameof(Conjunction)];
        Conjunction? conjunctionValue = Enum.TryParse<Conjunction>(conjunctionCapture.Value, true, out var parsed) ? parsed : null;

        var manyTokenType = typeof(ManyOf<>).MakeGenericType(_baseType);
        var manyPropVal = Activator.CreateInstance(manyTokenType, hydratedItems, conjunctionValue, conjunctionCapture);

        // Use the capture for the entire ManyOf group.
        var manyOfCapture = match.Groups[Name].Success ? match.Groups[Name] : match;
        token.SetPropertyFromCapture(RegexPropInfo, manyOfCapture, manyPropVal);

        return true;
    }

    public override string ToString() => base.ToString();
}