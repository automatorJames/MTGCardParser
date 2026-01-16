namespace MTGPlexer.RegexGeneration.RegexSegments;

public record DynamicOfSegment : ScalarCaptureSegmentBase
{
    public DynamicOfSegment(TemplatePropInfo captureProp) : base(captureProp)
    {
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenGroup(TemplatePropInfo);
        builder.AddAlternateValues(ScalarAlternativeSet.Alternates);
        builder.CloseGroup();
    }

    protected override void SetScalarAlternativeSet(TemplatePropInfo captureProp)
    {
        var captureAlternatives =
            captureProp.Prop.GetCustomAttribute<RegexPatternAttribute>()?.Patterns // Respect RegexPattern override if present
            ?? [@"[^.]+"];                                                             // Otherwise default to "one or more non-period characters"
            
        ScalarAlternativeSet = new(captureAlternatives.ToList());
    }

    public override object GetValueToSet(TokenUnit parentTokenUnit, Group namedGroup)
    {
        // Though required to be overridden to fulfill the base contract, this code is never reached 
        // because dynamic captures props are pre-filled by the tokenizer rather than upon instantiation of the
        // parent token.

        return null;
    }

    public bool SetValueFromPrefilledDynamicToken(TokenUnit token, object prefilledValue)
    {
        var genericType = TemplatePropInfo.Prop.PropertyType.GenericTypeArguments[0];

        if (!genericType.IsAssignableTo(typeof(TokenUnit)) || prefilledValue is not TokenUnit prefilledTokenUnitValue)
            throw new NotImplementedException($"Haven't yet implemented support for dynamic captures of types of other than TokenUnit types");

        var closedType = typeof(DynamicOf<>).MakeGenericType(genericType);
        var dynamicTokenInstance = Activator.CreateInstance(closedType, prefilledValue, prefilledTokenUnitValue.Match.RootMatch);
        token.SetPropertyFromCapture(TemplatePropInfo, prefilledTokenUnitValue.Match.RootMatch, dynamicTokenInstance);

        return true;
    }

}

