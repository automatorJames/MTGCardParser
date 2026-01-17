namespace MTGPlexer.RegexGeneration.RegexSegments;

public record DynamicOfSegment : ScalarCaptureSegmentBase
{
    CaptureGroupSegmentBase _regexProp;

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
        var capture = new ExtractedCapture(prefilledTokenUnitValue.Match.RootMatch);
        PolyItemCapture polyItemCapture = new(prefilledValue, capture, TemplatePropInfo);
        var dynamicTokenInstance = Activator.CreateInstance(closedType, polyItemCapture, capture);
        token.SetPropertyFromCapture(TemplatePropInfo, capture, dynamicTokenInstance);

        return true;
    }

    public override object GetPropertyValue(MatchTraversalState parentTokenUnitMatch, ExtractedCapture scopedCapture)
    {
        // This type is a special case. Since the Tokenizer preemptively passes TokenUnit a Dictionary of dynamic segment values,
        // TokenUnit sets them directly instead of the "normal" way here. Therefore we return null, which will cause the base to 
        // effectively "no-op".
    
        return null;
    }

    //public override object GetPropertyValue(MatchTraversalState parentTokenUnitMatch, ExtractedCapture scopedCapture)
    //{
	//	var genericType = TemplatePropInfo.Prop.PropertyType.GenericTypeArguments[0];
    //
	//	if (!genericType.IsAssignableTo(typeof(TokenUnit)))
	//		throw new NotImplementedException($"Haven't yet implemented support for dynamic captures of types of other than TokenUnit types");
    //
	//	var childItem = _regexProp.GetPropertyValue(parentTokenUnitMatch, scopedCapture);
	//	PolyItemCapture hydratedItem = new(childItem, scopedCapture, TemplatePropInfo);
    //
	//	var closedType = typeof(DynamicOf<>).MakeGenericType(genericType);
	//	//var dynamicTokenInstance = Activator.CreateInstance(closedType, prefilledValue, prefilledTokenUnitValue.Match.RootMatch.UnderlyingMatchObject);
	//	return null;
    //}

}