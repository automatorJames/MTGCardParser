namespace MTGPlexer.SnippetHelpers;

public record Snippet
{
    public string Text { get; init; }
    public bool IsPlural { get; init; }
    public bool IsOptional { get; init; }
    public bool IsNoPrecedingSpace { get; init; }
    public Proptions Proptions { get; init; } = Proptions.None;

    public Snippet(string text, Proptions proptions = Proptions.None)
    {
        Text = text;
        Proptions = proptions;
        IsOptional = this is SnippetOptional;
        IsNoPrecedingSpace = this is SnippetNoPrecedingSpace or SnippetOptionalPlural;
    }

    // Implicitly create a Snippet from a string
    public static implicit operator Snippet(string str) => new(str);
}