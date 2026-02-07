namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class OptionalOfNode : WrapperNode
{
    public OptionalOfNode(RegexNode parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
        if (!GenericType.IsAssignableTo(typeof(TokenUnit)))
            throw new Exception($"{nameof(OptionalOfNode)} expects '{nameof(TokenUnit)}' type, but found '{GenericType.Name}' type");
    }

    public override void AppendRegexBricks(RegexCollector collector)
    {
        builder.OpenNamedGroup(this);
        GetTemplateNodeForType().ComposeRegexLines(builder);
        GroupQuantifier? groupQuantifier = GroupQuantifier.Optional;
        builder.CloseGroup(groupQuantifier);
    }

    protected override object GetWrapperValue(CaptureContext captureContext)
    {
        var scopedCaptureContext = captureContext[FullyQualifiedName + "_" + GenericType.Name];

        if (!scopedCaptureContext.Success)
            return null;

        AddNewWrappedNode(scopedCaptureContext);

        return CreateWrapperValue();
    }
}