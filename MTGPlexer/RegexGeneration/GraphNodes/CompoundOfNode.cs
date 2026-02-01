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

    public override object TryGetValue(CaptureDictionary captureDictionary, out CaptureValueResult result)
    {
        var captures = captureDictionary[FullyQualifiedName];

        // We expect two or more captures
        if (captures.Length <= 1)
        {
            result = CaptureValueResult.Exception;
            return null;
        }

        for (int i = 0; i < captures.Length; i++)
            AddNewWrappedNode(captures[i], ordinal: i);

        result = CaptureValueResult.FoundWithValue;
        return CreateWrapperValue();
    }
}