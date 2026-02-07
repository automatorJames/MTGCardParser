namespace MTGPlexer.RegexGeneration.GraphNodes;

public abstract class BranchNode : NamedGroupNode
{
    public Dictionary<Type, List<RegexNode>> ChildrenPerType { get; } = [];
    public virtual List<RegexNode> Children => ChildrenPerType.First().Value;
    public virtual List<NamedGroupNode> NamedGroupNodes => Children.OfType<NamedGroupNode>().ToList();

    protected BranchNode(RegexNode parentNode, INavigable navigable) : base(parentNode, navigable)
    {
        // todo: I don't like this one bit. There should be a cleaner way to effect the outcome "DynamicOf won't have children".
        // The reason we don't let children get set is because (usually) DynamicOf<T> has T of TokenUnit, and since the GetChildNodes
        // instantiates T to get its Snippets, T = TokenUnit throws an error because it's an abstract type.
        if (UnderlyingType == typeof(TokenUnit) || GenericTypes.Any(x => x == typeof(TokenUnit)))
            return;

        if (GenericTypes.Any())
            foreach (var genericType in GenericTypes)
                ChildrenPerType[genericType] = GetChildNodes(genericType);
        else
            ChildrenPerType[UnderlyingType] = GetChildNodes(UnderlyingType);
    }

    List<RegexNode> GetChildNodes(Type type)
    {
        var snippets = Snippet.GetSnippets(type);
        return snippets.Select(x => SnippetToNode(this, x)).ToList();
    }

    static RegexNode SnippetToNode(RegexNode parentNode, Snippet snippet)
    {
        if (snippet is PropertySnippet propertySnippet)
        {
            var underlyingType = Nullable.GetUnderlyingType(propertySnippet.Prop.PropertyType) ?? propertySnippet.Prop.PropertyType;

            return underlyingType switch
            {
                { IsEnum: true } => new EnumNode(parentNode, propertySnippet),
                { } t when t.IsAssignableTo(typeof(ManyOf)) => new ManyOfNode(parentNode, propertySnippet),
                { } t when t.IsAssignableTo(typeof(CompoundOf)) => new CompoundOfNode(parentNode, propertySnippet),
                { } t when t.IsAssignableTo(typeof(OneOf)) => new OneOfNode(parentNode, propertySnippet),
                { } t when t.IsAssignableTo(typeof(OptionalOf)) => new OptionalOfNode(parentNode, propertySnippet),
                { } t when t.IsAssignableTo(typeof(DynamicOf)) => new DynamicOfNode(parentNode, propertySnippet),
                { } t when t == typeof(DefaultUnmatchedString) => new TokenUnitOneOfNode(parentNode, propertySnippet),
                { } t when typeof(TokenUnitCompound).IsAssignableFrom(t) => new TokenUnitCompoundNode(parentNode, propertySnippet),
                { } t when typeof(TokenUnitOneOf).IsAssignableFrom(t) => new TokenUnitOneOfNode(parentNode, propertySnippet),
                { } t when typeof(TokenUnit).IsAssignableFrom(t) => new TokenUnitNode(parentNode, propertySnippet),
                { } t when t == typeof(bool) => new BoolNode(parentNode, propertySnippet),
                { } t when t == typeof(int) => new BoolNode(parentNode, propertySnippet),
                { } t when t == typeof(PrecursorCapture) => new IntNode(parentNode, propertySnippet),
                _ => throw new Exception($"{underlyingType} is not a valid {nameof(PropertySnippet)} type")
            };
        }
        else
            return new TextNode(parentNode, snippet.Text);
    }

    /// <summary>
    /// Validates the capture structure based on two rules:
    /// 1. There must be at least one CaptureNode present.
    /// 2. All CaptureNodes must form a single, contiguous block (no gaps allowed).
    /// </summary>
    /// <returns>True if exactly one contiguous group of CaptureNodes exists.</returns>
    public bool ValidateCapturePropertiesAreContiguous()
    {
        int groups = 0;
        bool inGroup = false;

        foreach (var node in Children)
        {
            if (node is NamedGroupNode)
            {
                // If we hit a capture and weren't already in a group, 
                // we've discovered a new "island"
                if (!inGroup)
                {
                    groups++;
                    inGroup = true;
                }
            }
            else
            {
                // Any non-capture node (TextNode, etc.) terminates the current group
                inGroup = false;
            }
        }

        // Returns true only if we found exactly one contiguous cluster
        return groups == 1;
    }

    public override object GetValueAndSetHydrationInfo(CaptureContext captureContext)
    {
        var scopedCaptureContext = captureContext[FullyQualifiedName];

        if (!scopedCaptureContext.Success)
            return null;

        var instance = (TokenUnit)Activator.CreateInstance(UnderlyingType);

        foreach (var captureNode in NamedGroupNodes)
        {
            // will return false only if an underlying property has AbortIfSetPropertyToNull == true
            // and the property value is null
            var setSuccessfully = captureNode.SetPropertyValue(scopedCaptureContext, instance);

            if (!setSuccessfully)
                return null;
        }

        CaptureValueHydrationInfo = new(this, scopedCaptureContext.Capture, instance);

        return instance;
    }

}