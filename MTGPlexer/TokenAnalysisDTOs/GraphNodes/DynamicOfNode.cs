
namespace MTGPlexer.RegexGeneration.RegexSegments;

public record DynamicOfNode : WrapperPropertyNode
{
    ScalarAlternateSet _scalarAlternativeSet;

	public DynamicOfNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
        if (!GenericType.IsAssignableTo(typeof(TokenUnit)))
            throw new Exception($"{nameof(DynamicOfNode)} only supports {nameof(TokenUnit)} types");

		if (TokenTypeRegistry.PropScalarAlternativeSets.TryGetValue(propertySnippet.ToTemplatePropInfo(), out var scalarAlternativeSet))
			_scalarAlternativeSet = scalarAlternativeSet;
		else
		{
			var captureAlternatives =
				propertySnippet.Prop.GetCustomAttribute<RegexPatternAttribute>()?.Patterns // Respect RegexPattern override if present
				?? [@"[^.]+"];                                                         // Otherwise default to "one or more non-period characters"

			_scalarAlternativeSet = new(captureAlternatives.ToList());
		}
	}

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenGroup(PropertySnippet.ToTemplatePropInfo());
        builder.AddAlternateValues(_scalarAlternativeSet.Alternates);
        builder.CloseGroup();
    }

    protected override object GetPropertyValue(Capture capture)
    {
        //var resolvedTokens = TokenTypeRegistry.ClassTokenizer.Tokenize(
        //    parentTokenUnitMatch.SourceText,
        //    scopeToType: GenericType,
        //    scopeStart: scopedCapture.Index,
        //    scopeEnd: scopedCapture.Index + scopedCapture.Length);
        //
        //// Dynamic match tokens must not begin with DefaultUnmatchedString, and must contain at least one non-DefaultUnmatchedString
        //if (resolvedTokens.FirstOrDefault() is DefaultUnmatchedString || resolvedTokens.FirstOrDefault(x => x is not DefaultUnmatchedString) is not TokenUnit dynamicMatchToken)
        //{
        //    result = ValueResult.DynamicResolutionFailure;
        //    return null;
        //}
        //
        ////// Although the dynamic match must begin exactly where the parent match left off, it's allowed to be shorter than the remaining space in the parent.
        ////// When this occurs, shorten the parent match so that it ends where its dynamic child stops, allowing following tokens to be matched by something else.
        ////if (dynamicMatchToken.Match.AbsoluteEnd != match.Index + match.Length)
        ////	match = regex.Match(sourceText.FormattedText, currentIndex, dynamicMatchToken.Match.AbsoluteEnd - match.Index);
        //PolyItemCapture hydratedItem = new(dynamicMatchToken, scopedCapture, TemplatePropInfo);
        //var closedType = typeof(DynamicOf<>).MakeGenericType(GenericType);
        //var dynamicOfInstance = Activator.CreateInstance(closedType, hydratedItem, scopedCapture);
        //
        //result = ValueResult.Success;
        //return dynamicOfInstance;

        throw new NotImplementedException();
    }
}