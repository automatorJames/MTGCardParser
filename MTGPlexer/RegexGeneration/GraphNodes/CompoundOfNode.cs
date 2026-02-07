namespace MTGPlexer.RegexGeneration.GraphNodes;

public class CompoundOfNode : WrapperNode
{
    public CompoundOfNode(RegexNode parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
    }

    public override void AppendRegexBricks(RegexCollector collector)
    {
        builder.OpenNamedGroup(this);
        builder.OpenAnonymousGroup();
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