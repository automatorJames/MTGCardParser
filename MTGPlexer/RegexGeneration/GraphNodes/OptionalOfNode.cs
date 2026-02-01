namespace MTGPlexer.RegexGeneration.GraphNodes;

public class OptionalOfNode : WrapperNode
{
    public OptionalOfNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
        if (!GenericType.IsAssignableTo(typeof(TokenUnit)))
            throw new Exception($"{nameof(OptionalOfNode)} expects '{nameof(TokenUnit)}' type, but found '{GenericType.Name}' type");
    }

    public override void ComposeRegexLines(RegexBuilder builder)
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