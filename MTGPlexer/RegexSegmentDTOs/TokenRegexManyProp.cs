using MTGPlexer.RegexSegmentDTOs.RegexTemplateLines;

namespace MTGPlexer.RegexSegmentDTOs;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also some TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public class TokenRegexManyProp : TokenRegexProp
{
    Regex _multiRegex;
    public TokenRegexManyProp(RegexPropInfo captureProp) : base(captureProp)
    {
        var template = TokenTypeRegistry.GetTypeTemplate(captureProp.BaseType);
        ChildSegments = template.RegexSegments;
    }

    //protected override void SetRegex(RegexPropInfo captureProp)
    //{
    //    var multiTemplate = TokenTypeRegistry.GetTypeTemplate(captureProp.UnderlyingType);
    //    _multiRegex = multiTemplate.Regex;
    //    RegexString = $"((?# {captureProp.Name}){multiTemplate.RegexStringNoCaptureGroups})";
    //}

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

    public override void ComposeRegexLines(List<RegexTemplateLine> lines, List<string> namePath, int indentation)
    {
        //namePath ??= [];
        //namePath.Add(RegexPropInfo.Name);
        //lines ??= [];
        //
        //// Get the lines for a single iteration of this multi capture prop's base type
        //List<RegexTemplateLine> linesForSingle = [];
        //base.ComposeRegexLines(linesForSingle, namePath, indentation);
        //
        //lines.Add(new NamedGroupOpen(RegexPropInfo.Name, string.Join('.', namePath), indentation));
        //
        ////var itemName = RegexPropInfo.Name + "_Item";
        //
        //
        //
        //xxx(, xxx*(, (?<{nameof(Conjunction)}>and|or)) xxx
        //(?<{genericType.Name}_Item>{singleRegex})(?:,? (?<{genericType.Name}_Item>{singleRegex}))*(?:,? (?<{nameof(Conjunction)}>and|or)) (?<{genericType.Name}_Item>{singleRegex})
    }

    public override string ToString() => base.ToString();
}
