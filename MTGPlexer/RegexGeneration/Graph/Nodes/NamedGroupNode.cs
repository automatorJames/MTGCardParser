namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public abstract class NamedGroupNode : GroupNode
{
    public CaptureValueHydrationInfo CaptureValueHydrationInfo { get; protected set; }
    protected override RegexBrick GroupOpenBrick => new(this, $"?<{FullyQualifiedName}>", FullyQualifiedName);
    public string FullyQualifiedName { get; }

    public NamedGroupNode(RegexNode parentNode, TypeNavigation navigation) 
        : base(parentNode, navigation)
    {
        FullyQualifiedName = string.Join("_", Lineage.Where(x => !x.IsCollapsible));
    }

    public static NamedGroupNode GetWrappedTokenUnitOrEnumNode(WrapperNode parentNode, Type typeToWrap, string groupNameAppendix)
    {
        var wrappedName = parentNode.Name + "_" + groupNameAppendix;
        TypeNavigation navigation = new(typeToWrap, wrappedName);

        return GetUnderlyingType(typeToWrap) switch
        {
            { IsEnum: true } => new EnumNode(parentNode, navigation),
            { } t when typeof(TokenUnitCompound).IsAssignableFrom(t) => new TokenUnitCompoundNode(parentNode, navigation),
            { } t when typeof(TokenUnitOneOf).IsAssignableFrom(t) => new TokenUnitOneOfNode(parentNode, navigation),
            { } t when typeof(TokenUnit).IsAssignableFrom(t) => new TokenUnitNode(parentNode, navigation),
            _ => throw new Exception($"'{typeToWrap}' is not an enum or a {nameof(TokenUnit)} type")
        };
    }

    //public bool SetPropertyValue(CaptureContext captureContext, TokenUnit parent)
    //{
    //    if (ConcreteProperty == null)
    //        throw new Exception($"{FullyQualifiedName} does not represent a concrete CLR property, so its value cannot be set");
    //
    //    var value = GetValueAndSetHydrationInfo(captureContext);
    //
    //    if (value == null && AbortIfSetPropertyToNull)
    //        return false;
    //
    //    ConcreteProperty.SetValue(parent, value);
    //
    //    return true;
    //}
}
