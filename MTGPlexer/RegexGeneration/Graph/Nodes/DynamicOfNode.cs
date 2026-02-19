
namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class DynamicOfNode : ScalarContainerNode
{
    const string _defaultCaptureAllCharsPattern = @"[^.]+";
    string[] _dynamicPatterns;

	public DynamicOfNode(RegexNode parentNode, Navigation navigation) 
        : base(parentNode, navigation)
    {
        if (navigation.GenericTypes.Length != 1 || !navigation.GenericTypes[0].IsAssignableTo(typeof(TokenUnit)))
            throw new Exception($"{nameof(DynamicOfNode)} expects exactly one generic type assignable to {nameof(TokenUnit)}");

        _dynamicPatterns = navigation.Patterns ?? [_defaultCaptureAllCharsPattern];
	}

    protected override void AddReflectedChildren(List<RegexNode> children) =>
        children.AddRange(
            _dynamicPatterns.Select((x, idx) => new ScalarNode(
                    parentNode: this,
                    name: $"{GetType().Name}_Pattern" + (idx > 0 ? $"_{idx}" : ""),
                    scalarValue: true,
                    regex: x
                )));

    public override object GetValueSingle(Capture capture)
    {
        var resolvedTokens = TokenTypeRegistry.ClassTokenizer.Tokenize(capture.Value, scopeToType: Navigation.GenericTypes[0]);
    
        // Dynamic match tokens must not begin with DefaultUnmatchedString, and must contain at least one non-DefaultUnmatchedString
        if (resolvedTokens.FirstOrDefault() is DefaultUnmatchedString || resolvedTokens.FirstOrDefault(x => x is not DefaultUnmatchedString) is not TokenUnit dynamicMatchToken)
            return null;
    
        var closedType = typeof(DynamicOf<>).MakeGenericType(Navigation.GenericTypes[0]);
        var dynamicOfInstance = Activator.CreateInstance(closedType, dynamicMatchToken);
    
        return dynamicOfInstance;
    }
}