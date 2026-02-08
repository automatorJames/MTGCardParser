namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public abstract class BranchNode : RegexNode
{
    public TypeNavigation Navigation { get; }
    public List<RegexNode> Children { get; }


    protected BranchNode(RegexNode parentNode, TypeNavigation navigation) 
        : base(parentNode, navigation.Name)
    {
        Navigation = navigation;
        Children = GetChildNodes();
    }

    /// <summary>
    /// By default, "Children" means "all snippets associated with a type", where snippets are
    /// either PropertySnippets (child properties) or regular Snippets (text). May be overridden
    /// by BranchNode types for whom "Children" carries a different connotation, like EnumNode,
    /// where "Child" means "enum member". The rationale for overloading the "Child" concept like
    /// this is to make BranchNode.Children the single point where descendants of the branch are
    /// called to append their regex content to the collector. In other words "Branch" means 
    /// "Node that may contain multiple child regex elements", not necessarily "CLR object with
    /// properties" (though sometimes that's true). Therefore this is a convention for building
    /// Regex, not to be confused as a convetion for hydrating CLR types, given that the Node system
    /// does both those things.
    /// </summary>
    protected virtual List<RegexNode> GetChildNodes()
    {
        var snippets = Snippet.GetSnippets(Navigation.UnderlyingType);
        List<RegexNode> list = [];

        foreach (var snippet in snippets)
        {
            if (snippet is PropertySnippet propertySnippet)
                list.Add(GetNodeForPropertySnippetType(this, propertySnippet));
            else
                list.Add(new TextNode(this, snippet.Text));
        }

        return list;
    }

    public override void AppendRegexBricks(RegexCollector collector)
    {
        Children.ForEach(x => x.AppendRegexBricks(collector));
    }

    static RegexNode GetNodeForPropertySnippetType(RegexNode parentNode, PropertySnippet propertySnippet)
    {
        TypeNavigation navigation = new(propertySnippet.Type, propertySnippet.Prop.Name, propertySnippet.Proptions);
        var underlyingType = GetUnderlyingType(propertySnippet.Type);

        return GetUnderlyingType(propertySnippet.Type) switch
        {
            { IsEnum: true } => new EnumNode(parentNode, navigation),
            { } t when t.IsAssignableTo(typeof(ManyOf)) => new ManyOfNode(parentNode, navigation),
            { } t when t.IsAssignableTo(typeof(CompoundOf)) => new CompoundOfNode(parentNode, navigation),
            { } t when t.IsAssignableTo(typeof(OneOf)) => new OneOfNode(parentNode, navigation),
            { } t when t.IsAssignableTo(typeof(OptionalOf)) => new OptionalOfNode(parentNode, navigation),
            { } t when t.IsAssignableTo(typeof(DynamicOf)) => new DynamicOfNode(parentNode, navigation),
            { } t when t == typeof(DefaultUnmatchedString) => new TokenUnitOneOfNode(parentNode, navigation),
            { } t when typeof(TokenUnitCompound).IsAssignableFrom(t) => new TokenUnitCompoundNode(parentNode, navigation),
            { } t when typeof(TokenUnitOneOf).IsAssignableFrom(t) => new TokenUnitOneOfNode(parentNode, navigation),
            { } t when typeof(TokenUnit).IsAssignableFrom(t) => new TokenUnitNode(parentNode, navigation),
            { } t when t == typeof(bool) => new BoolNode(parentNode, navigation),
            { } t when t == typeof(int) => new BoolNode(parentNode, navigation),
            { } t when t == typeof(PrecursorCapture) => new IntNode(parentNode, navigation),
            _ => throw new Exception($"{underlyingType} is not a valid {nameof(PropertySnippet)} type")
        };
    }

    //public override object GetValueAndSetHydrationInfo(CaptureContext captureContext)
    //{
    //    var scopedCaptureContext = captureContext[FullyQualifiedName];
    //
    //    if (!scopedCaptureContext.Success)
    //        return null;
    //
    //    var instance = (TokenUnit)Activator.CreateInstance(UnderlyingType);
    //
    //    foreach (var captureNode in NamedGroupNodes)
    //    {
    //        // will return false only if an underlying property has AbortIfSetPropertyToNull == true
    //        // and the property value is null
    //        var setSuccessfully = captureNode.SetPropertyValue(scopedCaptureContext, instance);
    //
    //        if (!setSuccessfully)
    //            return null;
    //    }
    //
    //    CaptureValueHydrationInfo = new(this, scopedCaptureContext.Capture, instance);
    //
    //    return instance;
    //}

    static Type GetUnderlyingType(Type type)
        => Nullable.GetUnderlyingType(type) ?? type;
}