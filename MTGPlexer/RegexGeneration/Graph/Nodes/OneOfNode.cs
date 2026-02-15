namespace MTGPlexer.RegexGeneration.Graph.Nodes;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also some TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public class OneOfNode : WrapperNode
{
    protected override Joiner Joiner => Joiner.Pipe;

    NamedGroupNode _nodeTheFirst;
    NamedGroupNode _nodeTheSecond;
    NamedGroupNode _nodeTheThird;

    List<RegexNode> _immediateChildren = [];

    public OneOfNode(RegexNode parentNode, PropNavigation navigation) 
        : base(parentNode, navigation)
    {
        SetChildNodes(navigation);
    }

    void SetChildNodes(PropNavigation navigation)
    {
        _nodeTheFirst = GetNamedGroupChild(this, navigation, GenericType, OneOfItemOrdinal.First.ToString());
        _nodeTheSecond = GetNamedGroupChild(this, navigation, GenericType, OneOfItemOrdinal.Second.ToString());

        _immediateChildren.AddRange([_nodeTheFirst, _nodeTheSecond]);

        if (GenericTypes.Length >= 3)
        {
            _nodeTheThird = GetNamedGroupChild(this, navigation, GenericType, OneOfItemOrdinal.Third.ToString());
            _immediateChildren.Add(_nodeTheThird);
        }
    }

    protected override List<RegexNode> GetChildNodes() => _immediateChildren;

    //protected override object GetWrapperValue(CaptureContext captureContext)
    //{
    //    int captureFoundAtGenericTypeIndex = -1;
    //
    //    for (int i = 0; i < GenericTypes.Length; i++)
    //    {
    //        var genericType = GenericTypes[i];
    //        var scopedCapture = captureContext[FullyQualifiedName + "_" + genericType.Name];
    //    
    //        if (!scopedCapture.Success)
    //            continue;
    //
    //        AddNewWrappedNode(scopedCapture, genericType: genericType);
    //        captureFoundAtGenericTypeIndex = i;
    //
    //        goto ItemHasBeenFound;
    //    }
    //   
    //    throw new Exception($"Failed to extract any value for OneOfProp");
    //    
    //    ItemHasBeenFound:;
    //    
    //    var genericTypeDefinition = GenericTypes.Length switch
    //    {
    //        2 => typeof(OneOf<,>),
    //        3 => typeof(OneOf<,,>),
    //        _ => throw new Exception($"One-of regex prop count of {GenericTypes.Length} not supported")
    //    };
    //    
    //    return CreateWrapperValue(WrappedValues.Single(), captureFoundAtGenericTypeIndex);
    //}
}