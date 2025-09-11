using MTGPlexer.RegexGeneration.Composers;
using MTGPlexer.RegexGeneration.RegexTemplateLines;

namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also some TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public class TokenRegexManyProp : CaptureGroupPropBase
{
    Type _baseType;
    List<RegexSegmentBase> _singleIterationSegments;
    static EnumRegexProp _conjunctionProp = (EnumRegexProp)(new RegexPropInfo(typeof(ManyToken).GetProperty(nameof(ManyToken.Conjunction)))).GetCaptureGroupPropBase();

    public override Regex MatchRegex => TokenTypeRegistry.ManyOfRegexes[_baseType];

    public TokenRegexManyProp(RegexPropInfo captureProp) : base(captureProp)
    {
        _baseType = captureProp.BaseType;

        if (!_baseType.IsAssignableTo(typeof(TokenUnit)) && !_baseType.IsEnum)
            throw new Exception($"TokenRegexManyProp base type may only be derived from TokenUnit or be an enum");

        // todo: handle the case where base type is enum instead of token type
        var template = TokenTypeRegistry.GetTypeTemplate(captureProp.BaseType);
        _singleIterationSegments = template.RegexSegments;
    }

    public override void ComposeRegexLines(RegexLineCollector collector)
    {
        collector.OpenGroup(RegexPropInfo, neverAddSpacesToGroupMembers: true);
        ConcatenatingComposer.Instance.Compose(collector, _singleIterationSegments);
        collector.OpenGroup(neverAddSpacesToGroupMembers: true);
        collector.AddTextLine(", ");
        ConcatenatingComposer.Instance.Compose(collector, _singleIterationSegments);
        collector.CloseGroup(GroupQuantifier.AnyNumber);
        collector.OpenGroup(neverAddSpacesToGroupMembers: true);
        collector.AddTextLine(", ");
        collector.OpenGroup(neverAddSpacesToGroupMembers: true);
        _conjunctionProp.ComposeRegexLines(collector);
        collector.AddTextLine(" ");
        collector.CloseGroup(GroupQuantifier.Optional);
        ConcatenatingComposer.Instance.Compose(collector, _singleIterationSegments);
        collector.CloseGroup();
        collector.CloseGroup();
    }

    public override bool SetValueFromMatch(TokenUnit token, StructuredMatch parentMatch)
    {
        var childMatch = parentMatch.GetChildMatch(this);
        
        var itemCaptures = parentMatch.Match.Groups[$"{RegexPropInfo.BaseType.Name}"]
                .Captures
                .ToList();

        List<object> hydratedItems = [];
        var baseTypeRegex = TokenTypeRegistry.Templates[RegexPropInfo.BaseType].Regex;
        CaptureGroupPropBase singleBaseProp = RegexPropInfo.GetCaptureGroupPropBase(forceGetUnderlyingPropType: true);

        foreach (var itemCapture in itemCaptures)
        {
            var childItem = parentMatch.GetChildSubCapture(singleBaseProp, itemCapture);
            hydratedItems.Add(childItem);
        }

        var conjunctionString = parentMatch.Match.Groups[nameof(Conjunction)].Value;
        var conjunctionValue = Enum.GetValues<Conjunction>().FirstOrDefault(x => x.ToString().Equals(conjunctionString, StringComparison.OrdinalIgnoreCase));
        var manyPropVal = Activator.CreateInstance(RegexPropInfo.UnderlyingType, hydratedItems, conjunctionValue);
        token.SetPropertyFromMatch(RegexPropInfo, parentMatch, manyPropVal);

        return true;
    }

    public override string ToString() => base.ToString();
}