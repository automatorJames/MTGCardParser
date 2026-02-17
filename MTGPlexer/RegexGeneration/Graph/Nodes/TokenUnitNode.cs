namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class TokenUnitNode : NamedGroupNode
{
    public bool IsRoot => ParentNode == null;
    protected override Joiner Joiner => Joiner.Space;

    public TokenUnitNode(RegexNode parentNode, TypeNavigation navigation) 
        : base(parentNode, navigation)
    {
    }

    protected override void AddComputedChildren(List<RegexNode> children)
    {
        var snippets = Snippet.GetSnippets(Navigation.UnderlyingType);

        foreach (var snippet in snippets)
        {
            if (snippet is PropertySnippet propertySnippet)
                children.Add(GetNodeForPropertySnippetType(this, propertySnippet));
            else
                children.Add(new TextNode(this, snippet.Text));
        }
    }

    static RegexNode GetNodeForPropertySnippetType(RegexNode parentNode, PropertySnippet propertySnippet)
    {
        PropNavigation navigation = new(propertySnippet);

        return navigation.UnderlyingType.GetUnderlyingType() switch
        {
            { IsEnum: true } => new EnumNode(parentNode, navigation),
            { } t when t.IsAssignableTo(typeof(ManyOf)) => new ManyOfNode(parentNode, navigation),
            { } t when t.IsAssignableTo(typeof(CompoundOf)) => new CompoundOfNode(parentNode, navigation),
            { } t when t.IsAssignableTo(typeof(OneOf)) => new OneOfNode(parentNode, navigation),
            { } t when t.IsAssignableTo(typeof(OptionalOf)) => new OptionalOfNode(parentNode, navigation),
            { } t when t.IsAssignableTo(typeof(DynamicOf)) => new DynamicOfNode(parentNode, navigation),
            { } t when t == typeof(DefaultUnmatchedString) => new UnmatchedTokenUnitNode(parentNode, navigation),
            { } t when typeof(TokenUnitCompound).IsAssignableFrom(t) => new TokenUnitCompoundNode(parentNode, navigation),
            { } t when typeof(TokenUnitOneOf).IsAssignableFrom(t) => new TokenUnitOneOfNode(parentNode, navigation),
            { } t when typeof(TokenUnit).IsAssignableFrom(t) => new TokenUnitNode(parentNode, navigation),
            { } t when t == typeof(bool) => new BoolNode(parentNode, navigation),
            { } t when t == typeof(int) => new IntNode(parentNode, navigation),
            _ => throw new Exception($"'{navigation.UnderlyingType}' is not a valid {nameof(PropertySnippet)} type")
        };
    }

    //public override object GetValueAndSetHydrationInfo(CaptureContext captureContext)
    //{
    //    var scopedCaptureContext = captureContext[FullyQualifiedName];
    //
    //    if (!scopedCaptureContext.Success)
    //        return null;
    //
    //    var instance = (TokenUnit)Activator.CreateInstance(UnderlyingType);
    //
    //    foreach (var captureNode in NamedGroupNodes)
    //    {
    //        // will return false only if an underlying property has AbortIfSetPropertyToNull == true
    //        // and the property value is null
    //        var setSuccessfully = captureNode.SetPropertyValue(scopedCaptureContext, instance);
    //
    //        if (!setSuccessfully)
    //            return null;
    //    }
    //
    //    CaptureValueHydrationInfo = new(this, scopedCaptureContext.Capture, instance);
    //
    //    return instance;
    //}

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
