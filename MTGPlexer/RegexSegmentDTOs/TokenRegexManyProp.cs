using MTGPlexer.RegexSegmentDTOs.Composers;
using MTGPlexer.RegexSegmentDTOs.RegexTemplateLines;

namespace MTGPlexer.RegexSegmentDTOs;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also some TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public class TokenRegexManyProp : CaptureGroupPropBase
{
    Regex _multiRegex;
    List<RegexSegmentBase> _singleIterationSegments;
    static EnumRegexProp _conjunctionProp = (EnumRegexProp)(new RegexPropInfo(typeof(ManyToken).GetProperty(nameof(ManyToken.Conjunction)))).GetCaptureGroupPropBase();

    public TokenRegexManyProp(RegexPropInfo captureProp) : base(captureProp)
    {
        var template = TokenTypeRegistry.GetTypeTemplate(captureProp.BaseType);
        _singleIterationSegments = template.RegexSegments;
    }

    public override bool SetValueFromMatchSpan(TokenUnit parentToken, TextSpan matchSpan)
    {
        var manyMatch = _multiRegex.Match(matchSpan.ToStringValue());
        var manyMatchSubSpan = GetTextSubSpan(matchSpan, manyMatch);

        var itemCaptures = manyMatch.Groups[$"{RegexPropInfo.BaseType.Name}_Item"]
                .Captures
                .ToList();

        List<object> hydratedItems = [];

        foreach (var itemCapture in itemCaptures)
        {
            var itemSubSpan = GetTextSubSpan(matchSpan, itemCapture);
            var hydratedItemInstance = TokenUnit.InstantiateFromMatchString(RegexPropInfo.BaseType, itemSubSpan.Value, parentToken, RegexPropInfo);
            hydratedItems.Add(hydratedItemInstance);
        }

        var conjunctionString = manyMatch.Groups[nameof(Conjunction)].Value;
        var conjunctionValue = Enum.GetValues<Conjunction>().FirstOrDefault(x => x.ToString().Equals(conjunctionString, StringComparison.OrdinalIgnoreCase));
        var propVal = Activator.CreateInstance(RegexPropInfo.UnderlyingType, hydratedItems, conjunctionValue);
        parentToken.SetPropertyCapture(RegexPropInfo, manyMatchSubSpan.Value, propVal);

        return true;
    }

    public override void ComposeRegexLines(RegexLineCollector collector)
    {
        collector.OpenGroup(RegexPropInfo, neverAddSpacesToGroupMembers: true);
        ConcatenatingComposer.Instance.Compose(collector, _singleIterationSegments);
        collector.OpenGroup();
        collector.AddTextLine(", ");
        ConcatenatingComposer.Instance.Compose(collector, _singleIterationSegments);
        collector.CloseGroup(GroupQuantifier.AnyNumber);
        collector.OpenGroup();
        collector.AddTextLine(", ");
        collector.OpenGroup();
        _conjunctionProp.ComposeRegexLines(collector);
        collector.AddTextLine(" ");
        collector.CloseGroup(GroupQuantifier.Optional);
        ConcatenatingComposer.Instance.Compose(collector, _singleIterationSegments);
        collector.CloseGroup();
        collector.CloseGroup();
    }

    public override string ToString() => base.ToString();
}