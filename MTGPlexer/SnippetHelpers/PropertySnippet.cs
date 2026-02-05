namespace MTGPlexer.SnippetHelpers;

public record PropertySnippet : Snippet, INavigable
{
    public PropertyInfo Prop { get; }
    public Type Type { get; }
    public string Name { get; }
    public Proptions Proptions { get; }

    public PropertySnippet(string text, PropertyInfo prop, Proptions proptions) : base(text)
    {
        Prop = prop;
        Proptions = proptions;
        Type = prop.PropertyType;

        var name = prop.Name;

        if (prop.PropertyType.IsAssignableTo(typeof(XOf)))
            name = prop.PropertyType.GetGenericTypeDefinition().BaseType.Name + name;

        Name = name;
    }

    public static PropertySnippet[] GetPropertySnippets(Type type) =>
     type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
        .Select(x => new PropertySnippet(x.Name, x, Proptions.None)).ToArray();
}