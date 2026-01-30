namespace MTGPlexer.RegexGeneration.GraphNodes;

public record CompoundOfNode : WrapperPropertyNode
{
    public List<WrappedNode> WrappedNodes { get; } = [];

    public CompoundOfNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenNamedGroup(this);
        builder.OpenAnonymousGroup(spaceDisposition: SpaceDisposition.DisallowedLocal);
        TemplateNodeForComposition.ComposeRegexLines(builder);
        builder.AddTextLine(" ?");
        builder.CloseGroup(GroupQuantifier.OneOrMore);
        builder.AddNegativeSpaceLookbehindBoundary();
        builder.CloseGroup();
    }

    public override object GetValue(Capture[] captures)
    {
        List<PolyItemCapture> hydratedItems = [];
        
        for (int i = 0; i < captures.Length; i++)
        {
            var ordinalCapture = captures[i];
            ExtractedCapture extractedCapture = new(ordinalCapture, FullyQualifiedName, i, captures.Length); // todo: deprecate ExtractedCapture?
            var childItem = TemplateNodeForComposition with { Ordinal = i };
            PolyItemCapture hydratedItem = new(childItem, extractedCapture);
            hydratedItems.Add(hydratedItem);
        }
        
        var compoundType = typeof(CompoundOf<>).MakeGenericType(GenericType);
        var compoundPropVal = Activator.CreateInstance(compoundType, hydratedItems);
        
        return compoundPropVal;
    }


    public override string ToString() => base.ToString();
}