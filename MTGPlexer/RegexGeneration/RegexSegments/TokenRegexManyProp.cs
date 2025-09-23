using MTGPlexer.RegexGeneration.Composers;
using MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;
using System.Collections;

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
    string _itemName;
    List<RegexSegmentBase> _singleIterationSegments;
    static EnumRegexProp _conjunctionProp = (EnumRegexProp)(new RegexPropInfo(typeof(ManyOf).GetProperty(nameof(ManyOf.Conjunction)))).GetCaptureGroupPropBase();

    public override Regex MatchRegex => TokenTypeRegistry.ManyOfRegexes[_baseType];

    public TokenRegexManyProp(RegexPropInfo captureProp) : base(captureProp)
    {
        _baseType = captureProp.BaseType;
        _itemName = $"{captureProp.Name}_item";

        if (!_baseType.IsAssignableTo(typeof(TokenUnit)) && !_baseType.IsEnum)
            throw new Exception($"TokenRegexManyProp base type may only be derived from TokenUnit or be an enum");

        if (_baseType.IsAssignableTo(typeof(TokenUnit)))
        {
            _manyItemType = ManyItemVariant.TokenUnit;
            var template = TokenTypeRegistry.GetTypeTemplate(captureProp.BaseType);
            _singleIterationSegments = template.RegexSegments;
        }
        else if (_baseType.IsEnum)
        {
            _manyItemType = ManyItemVariant.Enum;
            EnumRegexProp proxyEnumRegexProp = new(captureProp.DerviveForManyOfItem());
            _singleIterationSegments = [proxyEnumRegexProp];
        }
        else
            throw new Exception($"TokenRegexManyProp base type may only be derived from TokenUnit or be an enum");

    }

    public override void ComposeRegexLines(RegexLineCollector collector)
    {
        collector.OpenGroup(RegexPropInfo, spaceDisposition: SpaceDisposition.NeverAddSpaceLocal);
        ConcatenatingComposer.Instance.Compose(collector, _singleIterationSegments);
        collector.OpenGroup(spaceDisposition: SpaceDisposition.NeverAddSpaceLocal);
        collector.AddTextLine(", ");
        ConcatenatingComposer.Instance.Compose(collector, _singleIterationSegments);
        collector.CloseGroup(GroupQuantifier.AnyNumber);
        collector.OpenGroup(spaceDisposition: SpaceDisposition.NeverAddSpaceLocal);
        collector.AddTextLine(",? ");
        collector.OpenGroup(spaceDisposition: SpaceDisposition.NeverAddSpaceLocal);
        _conjunctionProp.ComposeRegexLines(collector);
        collector.AddTextLine(" ");
        collector.CloseGroup(GroupQuantifier.Optional);
        ConcatenatingComposer.Instance.Compose(collector, _singleIterationSegments);
        collector.CloseGroup();
        collector.CloseGroup();
    }

    public override bool SetValueFromMatch(TokenUnit token, Match match)
    {
        var itemCaptures = match.Groups[_itemName]
                .Captures
                .ToList();

        // Dynamically create the generic type for List<ManyItemCapture<T>>
        var manyItemCaptureType = typeof(ManyItemCapture<>).MakeGenericType(_baseType);
        var listType = typeof(List<>).MakeGenericType(manyItemCaptureType);
        var hydratedItems = (IList)Activator.CreateInstance(listType);

        for (int i = 0; i < itemCaptures.Count; i++)
        {
            Capture itemCapture = itemCaptures[i];
            object childItem = null;

            if (_manyItemType == ManyItemVariant.TokenUnit)
            {
                childItem = TokenUnit.HydrateFromMatch(_baseType, match, itemCapture);
            }
            else if (_manyItemType == ManyItemVariant.Enum)
            {
                foreach (var enumMemberRegex in TokenTypeRegistry.EnumMemberRegexes[_baseType])
                {
                    if (enumMemberRegex.Value.IsMatch(itemCapture.Value))
                    {
                        childItem = enumMemberRegex.Key;
                        break;
                    }
                }

                if (childItem == null)
                {
                    throw new Exception($"Found no matching values for enum type '{_baseType.Name}' from capture '{itemCapture.Value}'");
                }
            }

            // Create an instance of ManyItemCapture<T> and add it to the list
            var hydratedItem = Activator.CreateInstance(manyItemCaptureType, childItem, itemCapture, i, RegexPropInfo);
            hydratedItems.Add(hydratedItem);
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