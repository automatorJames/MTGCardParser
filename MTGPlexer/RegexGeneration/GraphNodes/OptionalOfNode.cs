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

    public override object TryGetValue(CaptureDictionary captureDictionary, out CaptureValueResult result)
    {
        var capture = captureDictionary[FullyQualifiedName + "_" + GenericType.Name].SingleOrDefault();

        if (capture == null)
        {
            result = CaptureValueResult.NameNotFound;
            return null;
        }

        AddNewWrappedNode(capture);

        result = CaptureValueResult.FoundWithValue;
        return CreateWrapperValue();
    }
}