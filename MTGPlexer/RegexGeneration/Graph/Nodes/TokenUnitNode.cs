namespace MTGPlexer.RegexGeneration.Graph.Nodes;

/// <summary>
/// Represents a <see cref="TokenUnit"/> type (root or nested): its children mirror the type's declared
/// <see cref="Snippet"/>s, one child per literal text snippet or per property snippet's own node type.
/// </summary>
public class TokenUnitNode : NamedGroupNode
{
    protected override Joiner Joiner { get; }
    public override CaptureNodeType NodeType => CaptureNodeType.Token;

    public TokenUnitNode(RegexNode parentNode, Navigation navigation)
        : base(parentNode, navigation)
    {
        Joiner = Navigation.TokenTypeConfiguration.Joiner;
    }

    protected override void AddReflectedChildren(List<RegexNode> children)
    {
        foreach (var snippet in Navigation.TokenTypeConfiguration.Snippets)
            if (snippet is PropertySnippet propertySnippet)
                children.Add(GetNodeForPropertySnippetType(this, propertySnippet));
            else
                children.Add(new TextNode(this, snippet.Text));
    }

    /// <summary>Picks the concrete <see cref="RegexNode"/> subtype for a property snippet, based on its underlying CLR type (enum, bool, int, nested token unit, etc.).</summary>
    protected static RegexNode GetNodeForPropertySnippetType(RegexNode parentNode, PropertySnippet propertySnippet)
    {
        Navigation navigation = new(propertySnippet);

        return navigation.NodeType switch
        {
            { IsEnum: true } => new EnumNode(parentNode, navigation),
            { } t when t == typeof(UnmatchedString) => new UnmatchedTokenUnitNode(parentNode, navigation),
            { } t when typeof(TokenUnitOneOf).IsAssignableFrom(t) => new TokenUnitOneOfNode(parentNode, navigation),
            { } t when typeof(DynamicToken).IsAssignableFrom(t) => new DynamicTokenNode(parentNode, navigation),
            { } t when typeof(TokenUnit).IsAssignableFrom(t) => new TokenUnitNode(parentNode, navigation),
            { } t when t == typeof(bool) => new BoolNode(parentNode, navigation),
            { } t when t == typeof(int) => new IntNode(parentNode, navigation),
            _ => throw new Exception($"'{navigation.NodeType}' is not a valid {nameof(PropertySnippet)} type")
        };
    }

    /// <summary>Instantiates this node's <see cref="TokenUnit"/> type and hydrates every child named-group property from <paramref name="captureInfo"/>'s <see cref="CaptureTrace.CaptureContext"/>. Returns false if any required child fails to hydrate.</summary>
    public virtual bool TryHydrate(CaptureTrace captureInfo, out CaptureUnit tokenUnit)
    {
        tokenUnit = null;
        var instance = (CaptureUnit)Activator.CreateInstance(Navigation.NodeType);
        var namedGroupNodeChildren = Children.OfType<NamedGroupNode>().ToList();

        foreach (var child in namedGroupNodeChildren)
        {
            var setResult = child.SetPropertyValue(instance, captureInfo.CaptureContext);

            if (!setResult)
                return false;
        }

        instance.CaptureContext = captureInfo.CaptureContext;
        tokenUnit = instance;

        return true;
    }

    /// <inheritdoc/>
    protected override object GetValue(CaptureTrace captureInfo)
    {
        TryHydrate(captureInfo, out var tokenUnit);
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
