namespace MTGPlexer.TokenAnalysisDTOs.GraphNodes;

public record RootNode : Node
{
    public Type RootType { get; }
    public BuiltRegex BuiltRegex { get; }
    public Snippet[] Snippets { get; }
    public List<Node> Children { get; } = [];

    public IEnumerable<CaptureNode> _captureChildren => Children.OfType<CaptureNode>();

    public RootNode(Type type) : base(null, type.Name)
    {
        Snippets = GetSnippets();
        RootType = type;
        Children = Snippets.Select(SnippetToNode).ToList();
        BuiltRegex = BuildRegex();
    }

    Snippet[] GetSnippets()
    {
        var instance = Activator.CreateInstance(RootType);

        if (instance is not TokenUnit tokenUnitInstance)
            throw new Exception($"Type '{RootType.Name}' does not derive from type '{nameof(TokenUnit)}'");

        var snippets = tokenUnitInstance.GetSnippets();

        if (snippets.Length == 0)
        {
            // If children pass no arguments or call the default parameterless base constructor,
            // we assume they want to construct snippets from their ordered properties. If no
            // properties exist, we assume they want to construct a single snippet from a pattern attribute,
            // or even the type name as a last-ditch fallback.

            var publicPropNames = RootType.GetPublicPropNames();

            if (publicPropNames.Length > 0)
                snippets = RootType.GetPublicPropNames().Select(x => (Snippet)x).ToArray();
            else if (RootType.GetCustomAttribute<RegexPatternAttribute>() is RegexPatternAttribute attr)
                snippets = attr.Patterns.Select(x => (Snippet)x).ToArray();
            else
                snippets = [RootType.Name.ToFriendlyCase(TitleDisplayOption.Lower)];

            if (snippets.Length == 0)
                throw new Exception($"Type '{RootType.Name}' has no snippets or valid properties");
        }

        return snippets;
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

    public TokenUnit Hydrate(Match match)
    {
        var instance = (TokenUnit)Activator.CreateInstance(RootType);

        foreach (var captureChild in _captureChildren)
            captureChild.SetPropertyValue(match, instance);

        return instance;
    }

    Node SnippetToNode(Snippet snippet)
    {
        if (snippet is PropertySnippet propertySnippet)
        {
            var underlyingType = Nullable.GetUnderlyingType(propertySnippet.Prop.PropertyType) ?? propertySnippet.Prop.PropertyType;

            return underlyingType switch
            {
                { IsEnum: true } => new EnumNode(this, propertySnippet),
                { } t when t.IsAssignableTo(typeof(ManyOf)) => new ManyOfNode(this, propertySnippet),
                { } t when t.IsAssignableTo(typeof(CompoundOf)) => new CompoundOfNode(this, propertySnippet),
                { } t when t.IsAssignableTo(typeof(OneOf)) => new OneOfNode(this, propertySnippet),
                { } t when t.IsAssignableTo(typeof(OptionalOf)) => new OptionalOfNode(this, propertySnippet),
                { } t when t.IsAssignableTo(typeof(DynamicOf)) => new DynamicOfNode(this, propertySnippet),
                { } t when typeof(TokenUnitOneOf).IsAssignableFrom(t) => new TokenUnitOneOfNode(this, propertySnippet),
                { } t when typeof(TokenUnit).IsAssignableFrom(t) => new TokenUnitNode(this, propertySnippet),
                { } t when t == typeof(bool) => new BoolNode(this, propertySnippet),
                { } t when t == typeof(PlaceholderCapture) => new PlaceholderNode(this, propertySnippet),
                _ => throw new Exception($"{underlyingType} is not a valid {nameof(PropertySnippet)} type")
            };
        }
        else
            return new TextNode(this, snippet);
    }
}