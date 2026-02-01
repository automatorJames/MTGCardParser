namespace MTGPlexer.RegexGeneration.GraphNodes;

public class DynamicOfNode : LeafNode
{
    Type _genericType;
    ScalarAlternateSet _scalarAlternativeSet;

	public DynamicOfNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
        var genericTypes = propertySnippet.Type.GenericTypeArguments;

        if (genericTypes.Length != 1 || !genericTypes[0].IsAssignableTo(typeof(TokenUnit)))
            throw new Exception($"{nameof(DynamicOfNode)} expects exactly one generic type assignable to {nameof(TokenUnit)}");

        _genericType = genericTypes[0];

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

    public override object GetValueSingleCapture(Capture capture)
    {
        var resolvedTokens = TokenTypeRegistry.ClassTokenizer.Tokenize(capture.Value, scopeToType: _genericType);

        // Dynamic match tokens must not begin with DefaultUnmatchedString, and must contain at least one non-DefaultUnmatchedString
        if (resolvedTokens.FirstOrDefault() is DefaultUnmatchedString || resolvedTokens.FirstOrDefault(x => x is not DefaultUnmatchedString) is not TokenUnit dynamicMatchToken)
            return null;

        var closedType = typeof(DynamicOf<>).MakeGenericType(_genericType);
        var dynamicOfInstance = Activator.CreateInstance(closedType, dynamicMatchToken);

        return dynamicOfInstance;
    }
}