namespace MTGPlexer.SnippetHelpers;

public record Snippet
{
    public string Text { get; init; }
    public bool IsPlural { get; init; }
    public bool IsOptional { get; init; }
    public bool IsNoPrecedingSpace { get; init; }

    public Snippet(string text)
    {
        Text = text;
        IsOptional = this is SnippetOptional;
        IsNoPrecedingSpace = this is SnippetNoPrecedingSpace or SnippetOptionalPlural;
    }

    // Implicitly create a Snippet from a string
    public static implicit operator Snippet(string str) => new(str);

    public static Snippet[] GetSnippets(Type type)
    {
        if (TokenTypeRegistry.TypeSnippets.TryGetValue(type, out var snippets))
            return snippets;

        if (type.IsAssignableTo(typeof(TokenUnit)))
        {
            var instance = (TokenUnit)Activator.CreateInstance(type);
            snippets = instance.GetSnippets();

            if (snippets.Length == 0)
            {
                var propertySnippets = PropertySnippet.GetPropertySnippets(type);

                if (propertySnippets.Length > 0)
                    snippets = propertySnippets;
                else if (type.GetCustomAttribute<RegexPatternAttribute>() is RegexPatternAttribute attr)
                    snippets = attr.Patterns.Select(x => new Snippet(x)).ToArray();
                else
                    snippets = [new Snippet(type.Name.ToFriendlyCase(TitleDisplayOption.Lower))];
            }
        }

        TokenTypeRegistry.TypeSnippets[type] = snippets;
        return snippets;
    }
}