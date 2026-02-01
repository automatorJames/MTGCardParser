namespace MTGPlexer.RegexGeneration.GraphNodes;

public class CompoundOfNode : WrapperPropertyNode
{
    public CompoundOfNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenNamedGroup(this);
        builder.OpenAnonymousGroup(spaceDisposition: SpaceDisposition.DisallowedLocal);
        GetTemplateNodeForType().ComposeRegexLines(builder);
        builder.AddTextLine(" ?");
        builder.CloseGroup(GroupQuantifier.OneOrMore);
        builder.AddNegativeSpaceLookbehindBoundary();
        builder.CloseGroup();
    }

    public override CaptureValueInfo GetCaptureValueInfo(CaptureDictionary captureDictionary)
    {
        var captures = captureDictionary[FullyQualifiedName];

        // We expect two or more captures
        if (captures.Length <= 1)
            return new(this, CaptureValueResult.Exception);
        
        for (int i = 0; i < captures.Length; i++)
        {
            var ordinalCapture = captures[i];
            var itemNode = GetTemplateNodeForType(ordinal: i, siblingCaptureCount: captures.Length);
            WrappedNodes.Add(itemNode);
        }
        
        var compoundType = typeof(CompoundOf<>).MakeGenericType(GenericType);
        var values = WrappedNodes.Select(x => x.GetCaputureValueInfo(c))
        var compoundPropVal = Activator.CreateInstance(compoundType, hydratedItems);
        
        return compoundPropVal;
    }


    public override string ToString() => base.ToString();

}