namespace MTGPlexer.RegexGeneration.RegexSegments;

public class DynamicRegexProp : ScalarCapturePropBase
{
    public DynamicRegexProp(RegexPropInfo captureProp) : base(captureProp)
    {
    }

    public override void ComposeRegexLines(RegexLineCollector collector)
    {
        collector.OpenGroup(RegexPropInfo);
        collector.AddAlternatiingValues(ScalarAlternativeSet.Alternatives);
        collector.CloseGroup();
    }

    public override bool SetValueFromMatch(TokenUnit token, Match match)
    {
        var genericType = RegexPropInfo.Prop.PropertyType.GenericTypeArguments[0];

        if (!genericType.IsAssignableTo(typeof(TokenUnit)))
            throw new NotImplementedException($"Haven't yet implemented support for dynamic captures of types of other than TokenUnit types");

        var capture = match.Groups[Name];
        var dynamicTokenValue = TokenTypeRegistry.ClassTokenizer.TokenizeSingleNonDefault(capture, match);

        if (dynamicTokenValue == null)
            return false;

        var closedType = typeof(DynamicCapture<>).MakeGenericType(genericType);
        var dynamicTokenInstance = Activator.CreateInstance(closedType, dynamicTokenValue, capture);
        token.SetPropertyFromCapture(RegexPropInfo, capture, dynamicTokenInstance);
        return true;
    }
}

