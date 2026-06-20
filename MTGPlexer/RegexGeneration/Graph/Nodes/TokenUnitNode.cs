namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class TokenUnitNode : NamedGroupNode
{
    public bool IsRoot => ParentNode == null;
    protected override Joiner Joiner { get; }
    protected override GroupQuantifier? Quantifier { get; }

    public override CaptureNodeType NodeType => CaptureNodeType.TokenUnit;

    public TokenUnitNode(RegexNode parentNode, Navigation navigation) 
        : base(parentNode, navigation)
    {
        Joiner = Navigation.TokenTypeConfiguration.Joiner;
        Quantifier = Navigation.TokenTypeConfiguration.Quantifier;
    }

    protected override void AddReflectedChildren(List<RegexNode> children)
    {
        foreach (var snippet in Navigation.TokenTypeConfiguration.Snippets)
        {
            if (snippet is PropertySnippet propertySnippet)
                children.Add(GetNodeForPropertySnippetType(this, propertySnippet));
            else
                children.Add(new TextNode(this, snippet.Text));
        }
    }

    protected static RegexNode GetNodeForPropertySnippetType(RegexNode parentNode, PropertySnippet propertySnippet)
    {
        Navigation navigation = new(propertySnippet);

        return navigation.NodeType switch
        {
            { IsEnum: true } => new EnumNode(parentNode, navigation),
            { } t when t == typeof(DefaultUnmatchedString) => new UnmatchedTokenUnitNode(parentNode, navigation),
            { } t when typeof(TokenUnitOneOf).IsAssignableFrom(t) => new TokenUnitOneOfNode(parentNode, navigation),
            { } t when typeof(TokenUnit).IsAssignableFrom(t) => new TokenUnitNode(parentNode, navigation),
            { } t when t == typeof(bool) => new BoolNode(parentNode, navigation),
            { } t when t == typeof(int) => new IntNode(parentNode, navigation),
            _ => throw new Exception($"'{navigation.NodeType}' is not a valid {nameof(PropertySnippet)} type")
        };
    }

    public virtual bool TryHydrate(CaptureContext captureContext, out TokenUnit tokenUnit)
    {
        tokenUnit = null;
        var instance = (TokenUnit)Activator.CreateInstance(Navigation.NodeType);
        var namedGroupNodeChildren = Children.OfType<NamedGroupNode>().ToList();

        foreach (var child in namedGroupNodeChildren)
        {
            var setResult = child.SetPropertyValue(instance, captureContext);

            if (!setResult)
                return false;
        }

        instance.CaptureContext = captureContext;
        tokenUnit = instance;

        return true;
    }

    protected override object GetValue(CaptureInfo captureInfo)
    {
        TryHydrate(captureInfo.CaptureContext, out var tokenUnit);
        captureInfo.ClrValue = tokenUnit;

        return tokenUnit;
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
}
