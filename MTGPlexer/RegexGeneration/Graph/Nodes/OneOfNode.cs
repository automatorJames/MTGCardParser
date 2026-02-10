namespace MTGPlexer.RegexGeneration.Graph.Nodes;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also some TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public class OneOfNode : WrapperNode
{
    NamedGroupNode _itemTheFirst;
    NamedGroupNode _itemTheSecond;
    NamedGroupNode _itemTheThird;

    public OneOfNode(RegexNode parentNode, TypeNavigation navigation) 
        : base(parentNode, navigation)
    {
        _itemTheFirst = GetWrappedTokenUnitOrEnumNode(this, navigation.Type, OneOfItemOrdinal.First.ToString());
        _itemTheSecond = GetWrappedTokenUnitOrEnumNode(this, navigation.Type, OneOfItemOrdinal.Second.ToString());
        _itemTheThird = GetWrappedTokenUnitOrEnumNode(this, navigation.Type, OneOfItemOrdinal.Third.ToString());
    }

    protected override List<RegexNode> GetChildNodes()
    {
        List<RegexNode> children = [_itemTheFirst, _itemTheSecond];

        if (GenericTypes.Length >= 3)
            children.Add(_itemTheThird);

        return children;
    }

    public override void AppendRegexBricks(RegexCollector collector)
    {
        collector.Append(GroupOpenBrick);
        {
            collector.AppendJoined(Children, GetJoinerBrick(Joiner.Pipe));
        }
        collector.Append(GroupCloseBrick);
    }

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