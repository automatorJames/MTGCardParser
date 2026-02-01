namespace MTGPlexer.RegexGeneration.GraphNodes;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also some TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public class OneOfNode : WrapperNode
{
    public OneOfNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        var typedWrappedNodes = GenericTypes
            .Select((type, idx) => GetTemplateNodeForType(genericTypeIndex: idx))
            .Cast<Node>()
            .ToList();

        builder.OpenNamedGroup(this, spaceDisposition: SpaceDisposition.DisallowedLocal);
        AlternatingComposer.Instance.Compose(builder, typedWrappedNodes);
        builder.CloseGroup();
    }

    public override object TryGetValue(CaptureDictionary captureDictionary, out CaptureValueResult result)
    {
        int captureFoundAtGenericTypeIndex = -1;

        for (int i = 0; i < GenericTypes.Length; i++)
        {
            var genericType = GenericTypes[i];
            var capture = captureDictionary[FullyQualifiedName + "_" + genericType.Name].SingleOrDefault();
        
            if (capture == null)
                continue;

            AddNewWrappedNode(capture, genericType: genericType);
            captureFoundAtGenericTypeIndex = i;

            goto ItemHasBeenFound;
        }
       
        throw new Exception($"Failed to extract any value for OneOfProp");
        
        ItemHasBeenFound:;
        
        var genericTypeDefinition = GenericTypes.Length switch
        {
            2 => typeof(OneOf<,>),
            3 => typeof(OneOf<,,>),
            _ => throw new Exception($"One-of regex prop count of {GenericTypes.Length} not supported")
        };
        
        result = CaptureValueResult.FoundWithValue;
        return CreateWrapperValue(WrappedValues.Single(), captureFoundAtGenericTypeIndex);
    }
}