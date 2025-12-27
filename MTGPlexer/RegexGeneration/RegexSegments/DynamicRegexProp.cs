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

    protected override void SetScalarAlternativeSet(RegexPropInfo captureProp)
    {
        var captureAlternatives =
            captureProp.Prop.GetCustomAttribute<RegexPatternAttribute>()?.Patterns // Respect RegexPattern override if present
            ?? [".+"];                                                             // Otherwise default to "one or more characters"
            
        ScalarAlternativeSet = new(captureAlternatives.ToList());
    }

    public override bool SetValueFromNamedGroupInMatch(TokenUnit token)
    {
        // Though required to be overridden to fulfill the base contract, this code is never reached 
        // because dynamic captures props are pre-filled by the tokenizer rather than upon instantiation of the
        // parent token.

        return true;
    }

    public bool SetValueFromPrefilledDynamicToken(TokenUnit token, object prefilledValue)
    {
        var genericType = RegexPropInfo.Prop.PropertyType.GenericTypeArguments[0];

        if (!genericType.IsAssignableTo(typeof(TokenUnit)) || prefilledValue is not TokenUnit prefilledTokenUnitValue)
            throw new NotImplementedException($"Haven't yet implemented support for dynamic captures of types of other than TokenUnit types");

        var closedType = typeof(DynamicCapture<>).MakeGenericType(genericType);
        var dynamicTokenInstance = Activator.CreateInstance(closedType, prefilledValue, prefilledTokenUnitValue.Match.RegexMatch);
        token.SetPropertyFromCapture(RegexPropInfo, prefilledTokenUnitValue.Match.RegexMatch, dynamicTokenInstance);

        return true;
    }

}

