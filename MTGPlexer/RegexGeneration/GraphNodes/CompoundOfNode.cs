namespace MTGPlexer.RegexGeneration.GraphNodes;

public class CompoundOfNode : WrapperNode
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

    protected override object GetWrapperValue(CaptureContext captureContext)
    {
        var scopedCaptureContext = captureContext[FullyQualifiedName];

        // We expect two or more captures
        if (scopedCaptureContext.Captures.Length <= 1)
            return null;

        for (int i = 0; i < scopedCaptureContext.Captures.Length; i++)
        {
            var singleScopedContext = scopedCaptureContext.ScopeToCaptureIndex(i);
            AddNewWrappedNode(singleScopedContext, ordinal: i);
        }

        return CreateWrapperValue();
    }
}