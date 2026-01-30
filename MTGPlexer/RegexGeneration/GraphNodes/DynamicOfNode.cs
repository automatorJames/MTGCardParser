namespace MTGPlexer.RegexGeneration.GraphNodes;

public record DynamicOfNode : WrapperPropertyNode
{
    ScalarAlternateSet _scalarAlternativeSet;

	public DynamicOfNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
        if (!GenericType.IsAssignableTo(typeof(TokenUnit)))
            throw new Exception($"{nameof(DynamicOfNode)} only supports {nameof(TokenUnit)} types");

        // todo: bring back cache lookup if appropriate
		//if (TokenTypeRegistry.PropScalarAlternativeSets.TryGetValue(propertySnippet.ToTemplatePropInfo(), out var scalarAlternativeSet))
		//	_scalarAlternativeSet = scalarAlternativeSet;
		//else
		{
			var captureAlternatives =
				propertySnippet.Prop.GetCustomAttribute<RegexPatternAttribute>()?.Patterns // Respect RegexPattern override if present
				?? [@"[^.]+"];                                                         // Otherwise default to "one or more non-period characters"

			_scalarAlternativeSet = new(captureAlternatives.ToList());
		}
	}

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenNamedGroup(this);
        builder.AddAlternateValues(_scalarAlternativeSet.Alternates);
        builder.CloseGroup();
    }

    public override object GetValue(Capture capture)
    {
        var resolvedTokens = TokenTypeRegistry.ClassTokenizer.Tokenize(capture.Value, scopeToType: GenericType);
        
        // Dynamic match tokens must not begin with DefaultUnmatchedString, and must contain at least one non-DefaultUnmatchedString
        if (resolvedTokens.FirstOrDefault() is DefaultUnmatchedString || resolvedTokens.FirstOrDefault(x => x is not DefaultUnmatchedString) is not TokenUnit dynamicMatchToken)
            return null;

        ExtractedCapture extractedCapture = new(capture, FullyQualifiedName); // todo: deprecate ExtractedCapture?
        PolyItemCapture hydratedItem = new(dynamicMatchToken, extractedCapture);
        var closedType = typeof(DynamicOf<>).MakeGenericType(GenericType);
        var dynamicOfInstance = Activator.CreateInstance(closedType, hydratedItem, extractedCapture);
        
        return dynamicOfInstance;
    }
}