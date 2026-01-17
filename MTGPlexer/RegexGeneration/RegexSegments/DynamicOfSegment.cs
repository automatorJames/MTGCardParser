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

    public override object GetPropertyValue(MatchTraversalState parentTokenUnitMatch, Capture scopedCapture)
    {
        // This type is a special case. Since the Tokenizer preemptively passes TokenUnit a Dictionary of dynamic segment values,
        // TokenUnit sets them directly instead of the "normal" way here. Therefore we return null, which will cause the base to 
        // effectively "no-op".

        return null;
    }
}

