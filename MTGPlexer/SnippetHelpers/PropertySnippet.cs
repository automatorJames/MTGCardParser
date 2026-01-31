namespace MTGPlexer.SnippetHelpers;

public record PropertySnippet : Snippet
{
    public string Name { get; }
    public PropertyInfo Prop { get; }
    public Proptions Proptions { get; }

    public PropertySnippet(string text, PropertyInfo prop, Proptions proptions) : base(text)
    {
        Prop = prop;
        Proptions = proptions;

        var name = prop.Name;

        if (prop.PropertyType.IsAssignableTo(typeof(XOf)))
            name = prop.PropertyType.GetGenericTypeDefinition().BaseType.Name + name;

        Name = name;
    }
}