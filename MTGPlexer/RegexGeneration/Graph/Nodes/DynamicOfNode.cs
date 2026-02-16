
namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class DynamicOfNode : WrapperNode
{
    const string _defaultCaptureAllCharsPattern = @"[^.]+";
    string[] _dynamicPatterns;

    protected override bool OneOrMoreRegexPatternsRequired => true;
    protected override bool AbortIfSetPropertyToNull => true;

	public DynamicOfNode(RegexNode parentNode, PropNavigation navigation) 
        : base(parentNode, navigation)
    {
        if (GenericTypes.Length != 1 || GenericType.IsAssignableTo(typeof(TokenUnit)))
            throw new Exception($"{nameof(DynamicOfNode)} expects exactly one generic type assignable to {nameof(TokenUnit)}");

        _dynamicPatterns = navigation.Patterns ?? [_defaultCaptureAllCharsPattern];
	}

    protected override void AddComputedChildren(List<RegexNode> children) =>
        children.AddRange(
            _dynamicPatterns.Select((x, idx) => new ScalarNode(
                    parentNode: this,
                    name: $"{GetType().Name}-Pattern" + (idx > 0 ? $"-{idx}" : ""),
                    scalarValue: true,
                    regex: x
                )));

    //public override object GetValueSingle(Capture capture)
    //{
    //    var resolvedTokens = TokenTypeRegistry.ClassTokenizer.Tokenize(capture.Value, scopeToType: _genericType);
    //
    //    // Dynamic match tokens must not begin with DefaultUnmatchedString, and must contain at least one non-DefaultUnmatchedString
    //    if (resolvedTokens.FirstOrDefault() is DefaultUnmatchedString || resolvedTokens.FirstOrDefault(x => x is not DefaultUnmatchedString) is not TokenUnit dynamicMatchToken)
    //        return null;
    //
    //    var closedType = typeof(DynamicOf<>).MakeGenericType(_genericType);
    //    var dynamicOfInstance = Activator.CreateInstance(closedType, dynamicMatchToken);
    //
    //    return dynamicOfInstance;
    //}
}