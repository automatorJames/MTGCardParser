namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class DynamicOfNode : WrapperNode
{
    const string _defaultCaptureAllCharsPattern = @"[^.]+";
    string[] _captureAlternatives;

    protected override bool AbortIfSetPropertyToNull => true;

	public DynamicOfNode(RegexNode parentNode, PropNavigation navigation) 
        : base(parentNode, navigation)
    {
        if (GenericTypes.Length != 1 || GenericType.IsAssignableTo(typeof(TokenUnit)))
            throw new Exception($"{nameof(DynamicOfNode)} expects exactly one generic type assignable to {nameof(TokenUnit)}");

        _captureAlternatives = navigation.RegexPatterns ?? [_defaultCaptureAllCharsPattern];
	}

    protected override List<RegexNode> GetChildNodes() =>
        _captureAlternatives
        .Select((x, idx) => new ScalarNode(
            parentNode: this, 
            scalarValue: true, 
            name: "Dynamic-Capture",
            regex: x, 
            isFirst: idx == 0))
        .Cast<RegexNode>()
        .ToList();

    public override void AppendRegexBricks(RegexCollector collector)
    {
        collector.Append(GroupOpenBrick);
        collector.AppendJoined(Children, GetJoinerBrick(Joiner.Pipe));
        collector.Append(GroupCloseBrick);
    }

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