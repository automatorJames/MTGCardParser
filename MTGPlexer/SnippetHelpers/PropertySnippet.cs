namespace MTGPlexer.SnippetHelpers;

public record PropertySnippet : Snippet
{
    public PropertyInfo Prop { get; }
    public Proptions Proptions { get; }

    public PropertySnippet(string text, PropertyInfo prop, Proptions proptions) : base(text)
    {
        Prop = prop;
        Proptions = proptions;
    }
}