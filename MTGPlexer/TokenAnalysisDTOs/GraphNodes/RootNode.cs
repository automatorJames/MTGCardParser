namespace MTGPlexer.TokenAnalysisDTOs.GraphNodes;

public record RootNode : TypedNode
{
    public Type RootType { get; }
    public BuiltRegex BuiltRegex { get; }

    public IEnumerable<CaptureNode> CaptureChildren => Children.OfType<CaptureNode>();

    public RootNode(Type type) : base(null, type.Name, type)
    {
        RootType = type;
        BuiltRegex = BuildRegex();
    }

    BuiltRegex BuildRegex()
    {
        RegexBuilder regexBuilder = new(RootType);
        ComposeRegexLines(regexBuilder);
        return regexBuilder.GetBuiltRegex();
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        if (RootType.IsAssignableTo(typeof(TokenUnitOneOf)))
            AlternatingComposer.Instance.Compose(builder, Children);
        else if (RootType.IsAssignableTo(typeof(TokenUnit)))
            ConcatenatingComposer.Instance.Compose(builder, Children);
    }

    public TokenUnit Hydrate(CaptureDictionary captures)
    {
        var instance = (TokenUnit)Activator.CreateInstance(RootType);

        foreach (var captureChild in CaptureChildren)
            captureChild.SetPropertyValue(captures, instance);

        return instance;
    }
}