namespace MTGPlexer.RegexSegmentDTOs;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also some TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public record TokenRegexManyProp : RegexPropBase
{
    Regex _multiRegex;
    public TokenRegexManyProp(RegexPropInfo captureProp) : base(captureProp)
    {
    }
    
    public override bool SetValueFromMatchSpan(TokenUnit parentToken, TextSpan matchSpan)
    {
        var match = _multiRegex.Match(matchSpan.ToStringValue());

        var itemCaptures = match.Groups[$"{RegexPropInfo.BaseType.Name}_Item"]
                .Captures
                .ToList();

        List<object> hydratedItems = [];

        foreach (var itemCapture in itemCaptures)
        {
            var subSpan = GetTextSubSpan(matchSpan, itemCapture);
            var hydratedItemInstance = TokenUnit.InstantiateFromMatchString(RegexPropInfo.BaseType, subSpan.Value, parentToken, RegexPropInfo);
            hydratedItems.Add(hydratedItemInstance);
        }

        var conjunctionString = match.Groups[nameof(Conjunction)].Value;
        var conjunctionValue = Enum.GetValues<Conjunction>().FirstOrDefault(x => x.ToString().Equals(conjunctionString, StringComparison.OrdinalIgnoreCase));
        var propVal = Activator.CreateInstance(RegexPropInfo.UnderlyingType, hydratedItems, conjunctionValue);
        RegexPropInfo.Prop.SetValue(parentToken, propVal);

        return true;
    }

    protected override void SetRegex(RegexPropInfo captureProp)
    {
        var multiTemplate = TokenTypeRegistry.GetTypeTemplate(captureProp.UnderlyingType);
        _multiRegex = multiTemplate.Regex;
        RegexString = $"((?# {captureProp.Name}){multiTemplate.RegexStringNoCaptureGroups})";
    }

    public override string ToString() => base.ToString();
}
