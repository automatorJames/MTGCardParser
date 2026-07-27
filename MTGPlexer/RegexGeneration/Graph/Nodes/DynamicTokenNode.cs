
namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class DynamicTokenNode : TokenUnitNode
{
    const string _defaultCaptureAllCharsPattern = @"[^.]+";
    string[] _dynamicPatterns;

    public override CaptureNodeType NodeType => CaptureNodeType.DynamicOf;

    public DynamicTokenNode(RegexNode parentNode, Navigation navigation) 
        : base(parentNode, navigation)
    {
        _dynamicPatterns = navigation.Patterns ?? [_defaultCaptureAllCharsPattern];
	}

    protected override void AddReflectedChildren(List<RegexNode> children) =>
        children.AddRange(
            _dynamicPatterns.Select((x, idx) => new ScalarNode(
                    parentNode: this,
                    name: $"{GetType().Name}_Pattern" + (idx > 0 ? $"_{idx}" : ""),
                    scalarValue: true,
                    regex: x,
                    positionAmongSiblings: idx
                )));

    public override bool TryHydrate(CaptureContext captureContext, out TokenUnit tokenUnit)
    {
        tokenUnit = null;
        Type filterType = Navigation.Prop?.GetCustomAttribute<TypeFilterAttribute>()?.Type ?? typeof(TokenUnit);
        var resolvedTokens = TokenTypeRegistry.ClassTokenizer.Tokenize("discard a card", scopeToType: filterType);
        
        // Dynamic match tokens must not begin with DefaultUnmatchedString, and must contain at least one non-DefaultUnmatchedString
        if (resolvedTokens.FirstOrDefault() is DefaultUnmatchedString || resolvedTokens.FirstOrDefault(x => x is not DefaultUnmatchedString) is not TokenUnit dynamicMatchToken)
            return false;

        tokenUnit = new DynamicToken(dynamicMatchToken);

        return true;
    }

    //protected override object GetValue(CaptureInfo captureInfo)
    //{
    //    //var resolvedTokens = TokenTypeRegistry.ClassTokenizer.Tokenize(captureInfo.CaptureValue, scopeToType: Navigation.GenericTypes[0]);
    //    //
    //    //// Dynamic match tokens must not begin with DefaultUnmatchedString, and must contain at least one non-DefaultUnmatchedString
    //    //if (resolvedTokens.FirstOrDefault() is DefaultUnmatchedString || resolvedTokens.FirstOrDefault(x => x is not DefaultUnmatchedString) is not TokenUnit dynamicMatchToken)
    //    //    return null;
    //    //
    //    //var closedType = typeof(DynamicOf<>).MakeGenericType(Navigation.GenericTypes[0]);
    //    //var dynamicOfInstance = Activator.CreateInstance(closedType, dynamicMatchToken);
    //    //
    //    //return dynamicOfInstance;
    //
    //    return null;
    //}
}