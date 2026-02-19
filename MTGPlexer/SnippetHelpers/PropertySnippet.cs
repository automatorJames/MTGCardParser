namespace MTGPlexer.SnippetHelpers;

public record PropertySnippet : Snippet
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
        Name = prop.Name;

        // Extract metadata info from property attributes

        if (Prop.IsDefined(typeof(OneOrMoreAttribute)))
            Proptions |= Proptions.OneOrMore;

        if (Prop.IsDefined(typeof(OptionalAttribute)))
            Proptions |= Proptions.Optional;
    }

    public static PropertySnippet[] GetPropertySnippets(Type type) =>
        type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(x => x.GetSetMethod() != null) // Ignore get-only props like Joiner overrides
            .Select(x => new PropertySnippet(x.Name, x, Proptions.None))
            .ToArray();
}