namespace MTGPlexer.RegexGeneration.GraphNodes;

public abstract class TypedNode : ValueNode
{
    public Dictionary<Type, List<Node>> ChildrenPerType { get; } = [];
    public virtual List<Node> Children => ChildrenPerType.First().Value;
    public Type UnderlyingType { get; }
    public Type[] GenericTypes { get; }

    protected TypedNode(Node parentNode, string name, Type type) : base(parentNode, name)
    {
        UnderlyingType = Nullable.GetUnderlyingType(type) ?? type;
        GenericTypes = UnderlyingType.GetGenericArguments();

        // todo: I don't like this one bit. There should be a cleaner way to effect the outcome "DynamicOf won't have children".
        // The reason we don't let children get set is because (usually) DynamicOf<T> has T of TokenUnit, and since the GetChildNodes
        // instantiates T to get its Snippets, T = TokenUnit throws an error because it's an abstract type.
        if (UnderlyingType == typeof(TokenUnit) || GenericTypes.Any(x => x == typeof(TokenUnit)))
            return;

        if (GenericTypes.Any())
            foreach (var genericType in GenericTypes)
                ChildrenPerType[genericType] = GetChildNodes(genericType);
        else
            ChildrenPerType[UnderlyingType] = GetChildNodes(UnderlyingType);
    }

    List<Node> GetChildNodes(Type type)
    {
        var snippets = GetSnippets(type);
        return snippets.Select(x => SnippetToNode(this, x)).ToList();
    }

    static Snippet[] GetSnippets(Type type)
    {
        if (type.IsAssignableTo(typeof(TokenUnit)))
        {
            var instance = (TokenUnit)Activator.CreateInstance(type);
            var snippets = instance.GetSnippets();

            if (snippets.Length > 0)
                return snippets;
            else
            {
                var propertySnippets = GetPropertySnippets(type);

                if (propertySnippets.Length > 0)
                    return propertySnippets;
                else if (type.GetCustomAttribute<RegexPatternAttribute>() is RegexPatternAttribute attr)
                    return attr.Patterns.Select(x => new Snippet(x)).ToArray();
                else
                    snippets = [new Snippet(type.Name.ToFriendlyCase(TitleDisplayOption.Lower))];
            }

            return snippets;
        }

        return GetPropertySnippets(type);
    }

    static PropertySnippet[] GetPropertySnippets(Type type) =>
         type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Select(x => new PropertySnippet(x.Name, x, Proptions.None)).ToArray();

    static Node SnippetToNode(Node parentNode, Snippet snippet)
    {
        if (snippet is PropertySnippet propertySnippet)
        {
            var underlyingType = Nullable.GetUnderlyingType(propertySnippet.Prop.PropertyType) ?? propertySnippet.Prop.PropertyType;

            return underlyingType switch
            {
                { IsEnum: true } => new EnumNode(parentNode, propertySnippet),
                { } t when t.IsAssignableTo(typeof(ManyOf)) => new ManyOfNode(parentNode, propertySnippet),
                { } t when t.IsAssignableTo(typeof(CompoundOf)) => new CompoundOfNode(parentNode, propertySnippet),
                { } t when t.IsAssignableTo(typeof(OneOf)) => new OneOfNode(parentNode, propertySnippet),
                { } t when t.IsAssignableTo(typeof(OptionalOf)) => new OptionalOfNode(parentNode, propertySnippet),
                { } t when t.IsAssignableTo(typeof(DynamicOf)) => new DynamicOfNode(parentNode, propertySnippet),
                { } t when typeof(TokenUnitOneOf).IsAssignableFrom(t) => new TokenUnitOneOfNode(parentNode, propertySnippet),
                { } t when typeof(TokenUnit).IsAssignableFrom(t) => new TokenUnitNode(parentNode, propertySnippet),
                { } t when t == typeof(bool) => new BoolNode(parentNode, propertySnippet),
                { } t when t == typeof(PlaceholderCapture) => new PlaceholderNode(parentNode, propertySnippet),
                _ => throw new Exception($"{underlyingType} is not a valid {nameof(PropertySnippet)} type")
            };
        }
        else
            return new TextNode(parentNode, snippet.Text);
    }
}
