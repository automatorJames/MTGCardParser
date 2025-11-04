namespace MTGPlexer.RegexGeneration.RegexSegments;

public record DynamicRegexProp : ScalarCapturePropBase
{
    public DynamicRegexProp(RegexPropInfo captureProp) : base(captureProp)
    {
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenGroup(RegexPropInfo);
        builder.AddAlternateValues(ScalarAlternativeSet.Alternates);
        builder.CloseGroup();
    }

    public override bool SetValueFromMatch(TokenUnit token, Match match, int captureIndex, string distinguishingAppendix = null)
    {
        var genericType = RegexPropInfo.Prop.PropertyType.GenericTypeArguments[0];

        if (!genericType.IsAssignableTo(typeof(TokenUnit)))
            throw new NotImplementedException($"Haven't yet implemented support for dynamic captures of types of other than TokenUnit types");

        var group = match.Groups[Name + distinguishingAppendix];
        var capture = group.Captures[captureIndex];
        CaptureGroupPropPath ancestorCapturePath = new (token.CapturePath.PropPath.Dot(RegexPropInfo.Name));
        var dynamicTokenValue = TokenTypeRegistry.ClassTokenizer.TokenizeSingleNonDefaultChild(token, capture, match, ancestorCapturePath);

        if (dynamicTokenValue == null)
            return false;

        var closedType = typeof(DynamicCapture<>).MakeGenericType(genericType);
        var dynamicTokenInstance = Activator.CreateInstance(closedType, dynamicTokenValue, capture);
        token.SetPropertyFromCapture(RegexPropInfo, capture, dynamicTokenInstance);
        return true;
    }
}

